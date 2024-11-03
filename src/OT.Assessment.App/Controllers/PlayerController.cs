using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OT.Assessment.Tester.Data;
using OT.Assessment.Tester.Infrastructure;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;


namespace OT.Assessment.App.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChannel;
        private bool _disposed = false;

        public PlayerController(AppDbContext context)
        {
            _context = context;
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();
            _rabbitChannel.QueueDeclare(queue: "casinoWagerQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        //POST api/player/casinowager
        [HttpPost("casinoWager")]
        public async Task<IActionResult> CreateWager([FromBody] CasinoWager wager)
        {
            if (wager == null || wager.AccountId == Guid.Empty || wager.Amount <= 0)
            {
                return BadRequest("Invalid wager data.");
            }

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(wager));
            _rabbitChannel.BasicPublish(exchange: "", routingKey: "casinoWagerQueue", basicProperties: null, body: messageBody);

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _rabbitChannel?.Close();
                _rabbitConnection?.Close();
            }

            _disposed = true;
        }

        ~PlayerController()
        {
            Dispose(false);
        }
    }
}
