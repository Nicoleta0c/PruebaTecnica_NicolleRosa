using PruebaTecnica.Application.DTOs;
using PruebaTecnica.Application.Interfaces;
using PruebaTecnica.Domain.Entities;
using PruebaTecnica.Domain.Strategies;
using PruebaTecnica.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Services
{
    public class BetService

       //Crear interfaz IBetService y userService
    {
        private readonly UserService _userService;
        private readonly Random _random = new Random();

        public BetService(UserService userService)
        {
            _userService = userService;
        }

        // Generar resultado de la ruleta
        public RouletteResult GenerateSpin()
        {
            return new RouletteResult
            {
                Number = _random.Next(0, 37),
                Color = _random.Next(0, 2) == 0 ? "Red" : "Black"
            };
        }

        // agregar saldo
        public async Task AddBalanceAsync(string userName, decimal amountToAdd)
        {
            var user = await _userService.LoadUserAsync(userName);

            user.Balance += amountToAdd;

            await _userService.SaveUserAsync(user);
        }



        public async Task<BetResultDto> PlaceBetAsync(BetRequestDto request)
        {
            ValidateBet(request); 

            // Cargar usuario
            var user = await _userService.LoadUserAsync(request.UserName);
            if (user.Balance < request.Amount)
                throw new InvalidOperationException("Insufficient balance.");

            // resultado de la ruleta 
            //pudiera repetir logica pero para no romper DRY llamo al metodo que ya hice
            var rouletteResult = GenerateSpin();


            //Crear apuesta
            var bet = new Bet
            {
                Amount = request.Amount,
                BetType = Enum.Parse<BetType>(request.BetType),
                Color = request.Color,
                IsEven = request.IsEven,
                Number = request.Number
            };

            //Patron Strategies
            decimal amountWon = bet.BetType switch
            {
                BetType.Color => new ColorBetStrategy().CalculateWin(bet, rouletteResult),
                BetType.Parity => new ParityBetStrategy().CalculateWin(bet, rouletteResult),
                BetType.NumberAndColor => new NumberBetStrategy().CalculateWin(bet, rouletteResult),
                _ => -bet.Amount
            };

            //actualizar saldo
            user.Balance += amountWon;
            await _userService.SaveUserAsync(user);

            // resultado
            return new BetResultDto
            {
                UserName = request.UserName, 
                RouletteNumber = rouletteResult.Number,
                RouletteColor = rouletteResult.Color,
                AmountWon = amountWon,
                NewBalance = user.Balance
            };


        }

        // Validaciones de la solicitud de apuesta
        private void ValidateBet(BetRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Bet request cannot be null");

            if (string.IsNullOrWhiteSpace(request.UserName))
                throw new ArgumentException("User name is required", nameof(request.UserName));

            if (request.Amount <= 0)
                throw new ArgumentException("Bet amount must be greater than zero", nameof(request.Amount));

            if (!Enum.TryParse<BetType>(request.BetType, out var _))
                throw new ArgumentException($"Invalid bet type: {request.BetType}", nameof(request.BetType));

            switch (request.BetType)
            {
                case "Color":
                    if (string.IsNullOrWhiteSpace(request.Color))
                        throw new ArgumentException("Color must be specified for Color bet", nameof(request.Color));
                    break;

                case "Parity":
                    break;

                case "NumberAndColor":
                    if (request.Number < 0 || request.Number > 36)
                        throw new ArgumentException("Number must be between 0 and 36 for NumberAndColor bet.", nameof(request.Number));
                    if (string.IsNullOrWhiteSpace(request.Color))
                        throw new ArgumentException("Color must be specified for NumberAndColor bet.", nameof(request.Color));
                    break;

                default:
                    throw new ArgumentException($"Unsupported bet type: {request.BetType}", nameof(request.BetType));
            }
        }

    }
}