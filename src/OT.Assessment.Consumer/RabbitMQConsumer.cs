using OT.Assessment.App.Data;
using OT.Assessment.App.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json; 

public class RabbitMQConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private IConnection _connection;
    private IModel _channel; 

    // Constructor to initialize the RabbitMQ consumer, set up connection, and declare the queue
    public RabbitMQConsumer(IServiceProvider serviceProvider, ILogger<RabbitMQConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Configure and establish a connection to RabbitMQ server
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        try
        {
            // Declare the queue to ensure it exists before consuming messages
            _channel.QueueDeclare(queue: "casinoWagerQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _logger.LogInformation("Queue declared successfully.");
        }
        catch (Exception ex)
        {
            // Log any errors encountered during queue declaration
            _logger.LogError($"Failed to declare queue: {ex.Message}");
        }
    }

    // Main method for consuming messages from RabbitMQ
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a new consumer for the RabbitMQ channel
        var consumer = new EventingBasicConsumer(_channel);

        // Event handler for processing received messages
        consumer.Received += async (model, ea) =>
        {
            // Retrieve the message body and decode it as a string
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Deserialize the message into a CasinoWager object
            var wager = JsonSerializer.Deserialize<CasinoWager>(message);

            if (wager != null)
            {
                // Use a new DI scope to interact with the database
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
                    dbContext.CasinoWagers.Add(wager);
                    await dbContext.SaveChangesAsync(); 
                }
            }

            // Acknowledge message receipt to RabbitMQ
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        // Start consuming messages from the specified queue
        _channel.BasicConsume(queue: "casinoWagerQueue", autoAck: false, consumer: consumer);

        return Task.CompletedTask; 
    }

    // Clean up resources by closing the channel and connection
    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
