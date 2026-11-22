using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using SalesSystem.Models;

namespace TaxQueue.Messaging;

public sealed class CartPublisher : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public CartPublisher(RabbitMqOptions options)
    {
        _queueName = options.QueueName;
        _connection = RabbitMqConnectionFactory.Create(options);
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Publish(ShoppingCart cart)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cart, JsonOptions));
        var props = _channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2;

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _queueName,
            basicProperties: props,
            body: body);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
