using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesSystem.Models;
using SalesSystem.Services;

namespace TaxQueue.Messaging;

/// <summary>
/// Consome carrinhos da fila e calcula imposto com TaxCalculationService (reutilizado do UnitTests).
/// </summary>
public sealed class TaxConsumer : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        // PascalCase enum names ("Midwest") so the Svelte UI can send the same labels.
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly ITaxCalculationService _taxService;

    public TaxConsumer(RabbitMqOptions options, ITaxCalculationService taxService)
    {
        _taxService = taxService;
        _queueName = options.QueueName;
        _connection = RabbitMqConnectionFactory.Create(options);
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    public void Start()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnReceived;

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine($"Aguardando mensagens em '{_queueName}' (Ctrl+C para sair)...");
    }

    private void OnReceived(object? sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var cart = JsonSerializer.Deserialize<ShoppingCart>(json, JsonOptions)
                       ?? throw new InvalidOperationException("Payload inválido: carrinho nulo.");

            var result = _taxService.CalculateTotal(cart);

            Console.WriteLine("---------- imposto calculado ----------");
            Console.WriteLine($"Região:    {result.Region}");
            Console.WriteLine($"Alíquota:  {result.TaxRate:P0}");
            Console.WriteLine($"Subtotal:  {result.Subtotal:C}");
            Console.WriteLine($"Imposto:   {result.TaxAmount:C}");
            Console.WriteLine($"Total:     {result.Total:C}");
            Console.WriteLine("---------------------------------------");

            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Falha ao processar mensagem: {ex.Message}");
            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
