using OT.Assessment.App.Data;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OT.Assessment.App.Models.Interfaces;
using OT.Assessment.App.Infrastructure;
using OT.Assessment.App.Models;

namespace OT.Assessment.App.Adapters
{
    public class PlayerAdapter : IPlayerAdapter
    {
        private readonly AppDbContext _context;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChannel;
        private bool _disposed = false;

        public PlayerAdapter(AppDbContext context)
        {
            _context = context;
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();
            _rabbitChannel.QueueDeclare(queue: "casinoWagerQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task AddWagerAsync(CasinoWager wager)
        {
            // Check if the Provider is already tracked
            if (wager.Provider != null)
            {
                var existingProvider = await _context.Providers
                    .AsNoTracking() // Prevents tracking of the query result
                    .FirstOrDefaultAsync(p => p.ProviderId == wager.Provider.ProviderId);

                // If it exists, set the tracked provider to the wager
                if (existingProvider != null)
                {
                    wager.Provider = existingProvider;
                }
                else
                {
                    // Optionally attach if the provider is new
                    _context.Providers.Add(wager.Provider);
                }
            }

            // Check if the Player is already tracked
            if (wager.Player != null)
            {
                var existingPlayer = await _context.Players
                    .AsNoTracking() // Prevents tracking of the query result
                    .FirstOrDefaultAsync(p => p.AccountId == wager.Player.AccountId);

                // If it exists, set the tracked player to the wager
                if (existingPlayer != null)
                {
                    wager.Player = existingPlayer;
                }
                else
                {
                    // Optionally attach if the player is new
                    _context.Players.Add(wager.Player);
                }
            }

            // Add the wager to the context
            await _context.CasinoWagers.AddAsync(wager);
            await _context.SaveChangesAsync();
        }


        public async Task<(List<WagerDto> Data, int Total, int TotalPages)> GetWagersByPlayerAsync(Guid playerId, int page = 1, int pageSize = 10)
        {
            var query = _context.CasinoWagers
                .Where(w => w.AccountId == playerId)
                .Include(w => w.Game)
                .Include(w => w.Provider);

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            var wagers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WagerDto
                {
                    WagerId = w.WagerId,
                    Game = w.Game != null ? w.Game.Name : null, // Assuming Game.Name is the required property
                    Provider = w.Provider != null ? w.Provider.Name : null, // Assuming Provider.Name is the required property
                    Amount = w.Amount,
                    CreatedDate = w.CreatedDateTime
                })
                .ToListAsync();

            return (wagers, total, totalPages);
        }


        public async Task<List<object>> GetTopSpendersAsync(int count)
        {
            return await _context.CasinoWagers
                .GroupBy(w => w.AccountId)
                .Select(group => new
                {
                    AccountId = group.Key,
                    Username = group.Select(w => w.Username).ToString(),
                    TotalWagerAmount = group.Sum(w => w.Amount)
                })
                .OrderByDescending(player => player.TotalWagerAmount)
                .Take(count)
                .ToListAsync<object>();
        }

        public void PublishWager(CasinoWager wager)
        {
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(wager));
            _rabbitChannel.BasicPublish(exchange: "", routingKey: "casinoWagerQueue", basicProperties: null, body: messageBody);
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

        ~PlayerAdapter()
        {
            Dispose(false);
        }
    }
}
