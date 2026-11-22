using Microsoft.Extensions.Configuration;
using SalesSystem.Models;
using SalesSystem.Services;
using TaxQueue.Messaging;

namespace TaxQueue;

public static class Program
{
    public static int Main(string[] args)
    {
        var mode = args.FirstOrDefault()?.Trim().ToLowerInvariant() ?? "consume";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var rabbitOptions = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()
                            ?? new RabbitMqOptions();

        return mode switch
        {
            "publish" or "--publish" => RunPublish(rabbitOptions),
            "consume" or "--consume" => RunConsume(rabbitOptions),
            "help" or "--help" or "-h" => PrintHelp(),
            _ => UnknownMode(mode)
        };
    }

    private static int RunPublish(RabbitMqOptions options)
    {
        var cart = new ShoppingCart
        {
            Region = Region.Midwest,
            Items =
            [
                new CartItem
                {
                    Name = "Notebook",
                    UnitPrice = 2000m,
                    Quantity = 1,
                    IsTaxExempt = false
                },
                new CartItem
                {
                    Name = "Livro",
                    UnitPrice = 50m,
                    Quantity = 2,
                    IsTaxExempt = true
                }
            ]
        };

        using var publisher = new CartPublisher(options);
        publisher.Publish(cart);

        Console.WriteLine($"Carrinho publicado em '{options.QueueName}' (região {cart.Region}).");
        return 0;
    }

    private static int RunConsume(RabbitMqOptions options)
    {
        ITaxCalculationService taxService = new TaxCalculationService();
        using var consumer = new TaxConsumer(options, taxService);
        consumer.Start();

        var exit = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            exit.Set();
        };
        exit.Wait();
        return 0;
    }

    private static int PrintHelp()
    {
        Console.WriteLine("""
            TaxQueue — lab RabbitMQ para calcular imposto

            Uso:
              dotnet run --project TaxQueue -- consume   # escuta a fila e calcula imposto
              dotnet run --project TaxQueue -- publish   # envia um carrinho de exemplo

            RabbitMQ padrão: localhost:5672 (guest/guest), fila tax.calculate
            """);
        return 0;
    }

    private static int UnknownMode(string mode)
    {
        Console.Error.WriteLine($"Modo desconhecido: '{mode}'. Use publish, consume ou help.");
        return 1;
    }
}
