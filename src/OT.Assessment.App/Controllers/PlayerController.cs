using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OT.Assessment.Tester.Data;
using OT.Assessment.Tester.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;


namespace OT.Assessment.App.Controllers
{

    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlayerController(AppDbContext context)
        {
            _context = context;
        }

        //POST api/player/casinowager
        [HttpPost("casinoWager")]
        public async Task<IActionResult> CreateWager([FromBody] CasinoWager wager)
        {
            if (wager == null || wager.AccountId == Guid.Empty || wager.Amount <= 0)
            {
                return BadRequest("Invalid wager data.");
            }

            await _context.CasinoWagers.AddAsync(wager);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWagersByPlayer), new { playerId = wager.AccountId }, wager);
        }

        //GET api/player/{playerId}/wagers
        [HttpGet("{playerId}/wagers")]
        public async Task<IActionResult> GetWagersByPlayer(Guid playerId)
        {
            var playerWagers = await _context.CasinoWagers
                .Where(w => w.AccountId == playerId)
                .ToListAsync();

            if (!playerWagers.Any())
            {
                return NotFound("No wagers found for this player.");
            }

            return Ok(playerWagers);
        }


        //GET api/player/topSpenders?count=10
        [HttpGet("topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count = 10)
        {
            var topSpenders = await _context.CasinoWagers
                .GroupBy(w => w.AccountId)
                .Select(group => new
                {
                    AccountId = group.Key,
                    TotalWagerAmount = group.Sum(w => w.Amount)
                })
                .OrderByDescending(player => player.TotalWagerAmount)
                .Take(count)
                .ToListAsync();

            return Ok(topSpenders);
        }
    }
}
