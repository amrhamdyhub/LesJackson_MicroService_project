
using System.Text;
using CommandsServices.EventProcessing;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsServices.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName;
        public MessageBusSubscriber(
            IConfiguration configuration,
            IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ().GetAwaiter().GetResult();
        }

        private async Task InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]) };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            
            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            var queueDeclareOk = await _channel.QueueDeclareAsync();
            _queueName = queueDeclareOk.QueueName;

            await _channel.QueueBindAsync(queue: _queueName,
                exchange: "trigger",
                routingKey: "");

            Console.WriteLine("--> Listenting on the Message Bus...");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                if (_channel.IsOpen)
                {
                    _channel.CloseAsync();
                    _connection.CloseAsync();
                }
            });
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body.ToArray();
                var notificationMessage = Encoding.UTF8.GetString(body);

                await _eventProcessor.ProcessEvent(notificationMessage);
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
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

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            await Task.Run(() => Console.WriteLine("--> RabbitMQ Connection Shutdown"));
        }
    }
}
