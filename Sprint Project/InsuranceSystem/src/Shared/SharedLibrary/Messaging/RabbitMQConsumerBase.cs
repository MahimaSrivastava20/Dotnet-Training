using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SharedLibrary.Messaging;

public abstract class RabbitMQConsumerBase : BackgroundService
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly string _hostName;
    protected readonly IServiceProvider ServiceProvider;

    protected RabbitMQConsumerBase(IServiceProvider serviceProvider, string hostName = "localhost")
    {
        ServiceProvider = serviceProvider;
        _hostName = hostName;
    }

    protected abstract string QueueName { get; }
    protected abstract Task HandleMessageAsync(string message, IServiceScope scope);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        try
        {
            var factory = new ConnectionFactory { HostName = _hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                using var scope = ServiceProvider.CreateScope();
                try
                {
                    await HandleMessageAsync(message, scope);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };
            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        }
        catch (Exception)
        {
            // RabbitMQ not available - service starts without consuming
        }
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }

    protected static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json);
}
