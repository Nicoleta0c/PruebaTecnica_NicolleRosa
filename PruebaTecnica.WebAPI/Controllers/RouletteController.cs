using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.DTOs;
using PruebaTecnica.Application.Services;
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

        [HttpPost("place-bet")]
        public async Task<IActionResult> PlaceBet([FromBody] BetRequestDto betRequest)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(betRequest.Name))
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

        [HttpPost("add-balance")]
        public async Task<IActionResult> AddBalance([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(userDto.Name))
                return BadRequest("Invalid UserName.");

            if (userDto.Balance < 0)
                return BadRequest("Balance cannot be negative.");

            try
            {
                await _betService.AddBalanceAsync(userDto.Name, userDto.Balance.GetValueOrDefault());
                var user = await _userService.LoadUserAsync(userDto.Name);
                return Ok(new { UserName = user.Name, Balance = user.Balance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding balance: {ex.Message}");
            }
        }

        // Endepoint sin cuerpo para guardar user en la DB
        [HttpPost("save-balance/{name}")]
        public async Task<IActionResult> SaveBalance(string name, [FromBody] decimal balance)
        {
            try
            {
                await _userService.CommitUserToDbAsync(name);
                var user = await _userService.LoadUserAsync(name);
                return Ok(new { userName = name, balance = user.Balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("get-balance/{name}")]
        public async Task<IActionResult> GetBalance(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Invalid UserName.");

            try
            {
                var user = await _userService.LoadUserAsync(name);
                return Ok(new { UserName = user.Name, Balance = user.Balance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving balance: {ex.Message}");
            }
        }

        [HttpPost("clear-memory")]
        public IActionResult ClearMemory()
        {
            _userService.ClearMemory();
            return Ok("Memoria limpiada");
        }
    }
}