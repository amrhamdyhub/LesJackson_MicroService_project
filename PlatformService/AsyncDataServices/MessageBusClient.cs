using System.Text.Json;
using System.Text;
using PlatformService.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient , IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            SetupRabbitMqConnectionAsync().GetAwaiter().GetResult();
        }

        public async Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                await SendMessageAsync(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed, not sending");
            }
        }
        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }
        
        private async Task SetupRabbitMqConnectionAsync()
        {
            Console.WriteLine("--> Connecting to MessageBus");
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"] ?? throw new ArgumentNullException("RabbitMQHost"),
                Port = int.Parse(_configuration["RabbitMQPort"] ?? throw new ArgumentNullException("RabbitMQPort"))
            };
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var basicProperties = new BasicProperties();

            await _channel.BasicPublishAsync(
                exchange: "trigger",
                routingKey: "",
                mandatory: false,
                basicProperties: basicProperties,
                body: body);
            Console.WriteLine($"--> We have sent {message}");
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            await Task.Run(() => Console.WriteLine("--> RabbitMQ Connection Shutdown"));
        }
    }
}
