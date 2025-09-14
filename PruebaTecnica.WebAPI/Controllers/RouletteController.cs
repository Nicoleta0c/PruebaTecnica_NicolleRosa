using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.DTOs;
using PruebaTecnica.Application.Services;
using PruebaTecnica.Domain.Entities;  
using System;
using System.Threading.Tasks;

namespace PruebaTecnica.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        private readonly BetService _betService;
        private readonly UserService _userService;

        public RouletteController(BetService betService, UserService userService)
        {
            _betService = betService;
            _userService = userService;
        }

        
        [HttpGet("spin")]
        public IActionResult Spin()
        {
            var result = _betService.GenerateSpin();
            return Ok(new { Number = result.Number, Color = result.Color });
        }

        // guardar saldo o crear
        [HttpPost("save-balance")]
        public async Task<IActionResult> SaveBalance([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(userDto.UserName))
                return BadRequest("Invalid UserName.");

            if (userDto.Balance < 0)
                return BadRequest("Balance cannot be negative.");

            try
            {
            
                await _betService.AddBalanceAsync(userDto.UserName, userDto.Balance);
                // Traer el usuario actualizado
                var user = await _userService.LoadUserAsync(userDto.UserName);

                return Ok(new { UserName = user.Name, Balance = user.Balance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving balance: {ex.Message}");
            }
        }


        [HttpPost("place-bet")]
        public async Task<IActionResult> PlaceBet([FromBody] BetRequestDto betRequest)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(betRequest.UserName))
                return BadRequest("Invalid request data.");

            try
            {
                var result = await _betService.PlaceBetAsync(betRequest);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid bet: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}