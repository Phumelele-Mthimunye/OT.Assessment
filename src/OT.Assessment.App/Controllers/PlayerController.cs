using Microsoft.AspNetCore.Mvc;
using OT.Assessment.App.Models.Interfaces;
using OT.Assessment.Tester.Infrastructure;

namespace OT.Assessment.App.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("casinoWager")]
        public async Task<IActionResult> CreateWager([FromBody] CasinoWager wager)
        {
            var result = await _playerService.AddWagerAsync(wager);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetWagersByPlayer), new { playerId = result.Wager.AccountId }, result.Wager);
        }

        [HttpGet("{playerId}/wagers")]
        public async Task<IActionResult> GetWagersByPlayer(Guid playerId)
        {
            var result = await _playerService.GetWagersByPlayerAsync(playerId, 1, 10);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.WagerResponse);
        }

        [HttpGet("topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count = 10)
        {
            var result = await _playerService.GetTopSpendersAsync(count);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.TopSpenders);
        }
    }
}
