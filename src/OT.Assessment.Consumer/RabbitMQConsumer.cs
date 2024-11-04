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

    public RabbitMQConsumer(IServiceProvider serviceProvider, ILogger<RabbitMQConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        try
        {
            _channel.QueueDeclare(queue: "casinoWagerQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _logger.LogInformation("Queue declared successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to declare queue: {ex.Message}");
        }

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var wager = JsonSerializer.Deserialize<CasinoWager>(message);

            if (wager != null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.CasinoWagers.Add(wager);
                    await dbContext.SaveChangesAsync();
                }
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: "casinoWagerQueue", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
