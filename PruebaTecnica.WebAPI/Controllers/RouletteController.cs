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

        public RouletteController(BetService betService)
        {
            _betService = betService;
        }

        // Generar resultado de la ruleta
        [HttpGet("spin")]
        public IActionResult Spin()
        {
            var result = _betService.GenerateSpin();
            return Ok(new { Number = result.Number, Color = result.Color });
        }

        // Guardar saldo o crear usuario
        [HttpPost("save-balance")]
        public async Task<IActionResult> SaveBalance([FromBody] UserDto userDto)
        {
            try
            {
                await _betService.AddBalanceAsync(userDto.UserName, userDto.Balance);
                return Ok(new { UserName = userDto.UserName, Balance = userDto.Balance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving balance: {ex.Message}");
            }
        }

        // Previsualizar el resultado de la apuesta sin afectar el saldo
        [HttpPost("preview-bet")]
        public async Task<IActionResult> PreviewBet([FromBody] BetRequestDto betRequest)
        {
            try
            {
                var result = await _betService.PreviewBetAsync(betRequest);
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

        // Guardar el resultado de la apuesta (si gana o pierde)
        [HttpPost("commit-bet")]
        public async Task<IActionResult> CommitBet([FromBody] BetResultDto betResult)
        {
            try
            {
                await _betService.CommitBetAsync(betResult.UserName, betResult.NewBalance);
                return Ok(new { Message = "Bet committed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error committing bet: {ex.Message}");
            }
        }
    }
}
