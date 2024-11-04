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
    // The PlayerAdapter class implements the IPlayerAdapter interface
    public class PlayerAdapter : IPlayerAdapter
    {
        private readonly AppDbContext _context; // Database context for data operations
        private readonly IConnection _rabbitConnection; // RabbitMQ connection object
        private readonly IModel _rabbitChannel; // RabbitMQ channel for communication
        private bool _disposed = false; // Flag to track disposal of resources

        // Constructor for PlayerAdapter
        public PlayerAdapter(AppDbContext context)
        {
            _context = context; 

            // Set up RabbitMQ connection factory with hostname and credentials
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            _rabbitConnection = factory.CreateConnection(); // Create a new connection to RabbitMQ
            _rabbitChannel = _rabbitConnection.CreateModel(); // Create a channel for messaging

            // Declare a queue for casino wagers with specific settings
            _rabbitChannel.QueueDeclare(queue: "casinoWagerQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task AddWagerAsync(CasinoWager wager)
        {
            // Check if the Provider associated with the wager is already tracked
            if (wager.Provider != null)
            {
                // Look for an existing provider in the database
                var existingProvider = await _context.Providers
                    .AsNoTracking() 
                    .FirstOrDefaultAsync(p => p.ProviderId == wager.Provider.ProviderId);

                // If an existing provider is found, associate it with the wager
                if (existingProvider != null)
                {
                    wager.Provider = existingProvider;
                }
                else
                {
                    // If the provider is new, add it to the context
                    _context.Providers.Add(wager.Provider);
                }
            }

            // Check if the Player associated with the wager is already tracked
            if (wager.Player != null)
            {
                // Look for an existing player in the database
                var existingPlayer = await _context.Players
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.AccountId == wager.Player.AccountId);

                // If an existing player is found, associate it with the wager
                if (existingPlayer != null)
                {
                    wager.Player = existingPlayer;
                }
                else
                {
                    // If the player is new, add it to the context
                    _context.Players.Add(wager.Player);
                }
            }

            // Add the wager to the context and save changes to the database
            await _context.CasinoWagers.AddAsync(wager);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<WagerDto> Data, int Total, int TotalPages)> GetWagersByPlayerAsync(Guid playerId, int page = 1, int pageSize = 10)
        {
            // Query the casino wagers for the specified player
            var query = _context.CasinoWagers
                .Where(w => w.AccountId == playerId) // Filter by player's account ID
                .Include(w => w.Game)
                .Include(w => w.Provider); 

            // Calculate total number of wagers for pagination
            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)total / pageSize); 

            // Fetch the wagers with pagination
            var wagers = await query
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .Select(w => new WagerDto
                {
                    WagerId = w.WagerId,
                    Game = w.Game != null ? w.Game.Name : null, 
                    Provider = w.Provider != null ? w.Provider.Name : null,
                    Amount = w.Amount,
                    CreatedDate = w.CreatedDateTime
                })
                .ToListAsync();

            // Return the list of wagers, total count, and total pages
            return (wagers, total, totalPages);
        }

        public async Task<List<object>> GetTopSpendersAsync(int count)
        {
            return await _context.CasinoWagers
                .GroupBy(w => w.AccountId) // Group by AccountId to summarize spenders
                .Select(group => new
                {
                    AccountId = group.Key,
                    Username = group.Select(w => w.Username).ToString(),
                    TotalWagerAmount = group.Sum(w => w.Amount) 
                })
                .OrderByDescending(player => player.TotalWagerAmount) // Order by total wager amount descending
                .Take(count) // Take the top 'count' spenders
                .ToListAsync<object>();
        }

        public void PublishWager(CasinoWager wager)
        {
            // Serialize the wager object to JSON and encode it as a byte array
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(wager));
            // Publish the message to the RabbitMQ queue
            _rabbitChannel.BasicPublish(exchange: "", routingKey: "casinoWagerQueue", basicProperties: null, body: messageBody);
        }

        // Dispose method to clean up resources
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
