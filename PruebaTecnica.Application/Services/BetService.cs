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
    {
        private readonly UserService _userService;
        private readonly Random _random = new Random();

        public BetService(UserService userService)
        {
            _userService = userService;
        }

        public RouletteResult GenerateSpin()
        {
            return new RouletteResult
            {
                Number = _random.Next(0, 37),
                Color = _random.Next(0, 2) == 0 ? "Red" : "Black"
            };
        }

        //agregar balance
        public async Task AddBalanceAsync(string userName, decimal amountToAdd)
        {
            await _userService.AddBalanceAsync(userName, amountToAdd);
        }

        public async Task<BetResultDto> PlaceBetAsync(BetRequestDto request)
        {
            ValidateBet(request);

            var user = await _userService.LoadUserAsync(request.Name);
            if (user.Balance < request.Amount)
                throw new InvalidOperationException("Insufficient balance.");

            var rouletteResult = GenerateSpin();

            var bet = new Bet
            {
                Amount = request.Amount,
                BetType = Enum.Parse<BetType>(request.BetType),
                Color = request.Color,
                IsEven = request.IsEven,
                Number = request.Number
            };

            //patron strategy

            decimal amountWon = bet.BetType switch
            {
                BetType.Color => new ColorBetStrategy().CalculateWin(bet, rouletteResult),
                BetType.Parity => new ParityBetStrategy().CalculateWin(bet, rouletteResult),
                BetType.NumberAndColor => new NumberBetStrategy().CalculateWin(bet, rouletteResult),
                _ => -bet.Amount
            };

            //sumar balance + ganado y mostrar resultado

            user.Balance += amountWon;
            _userService.SaveUser(user);

            return new BetResultDto
            {
                Name = request.Name,
                RouletteNumber = rouletteResult.Number,
                RouletteColor = rouletteResult.Color,
                AmountWon = amountWon,
                NewBalance = user.Balance
            };
        }

        //validaciones de entrada
        private void ValidateBet(BetRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Bet request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("User name is required", nameof(request.Name));

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