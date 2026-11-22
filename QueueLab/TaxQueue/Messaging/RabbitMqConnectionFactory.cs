using RabbitMQ.Client;

namespace TaxQueue.Messaging;

public static class RabbitMqConnectionFactory
{
    public static IConnection Create(RabbitMqOptions options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            DispatchConsumersAsync = false
        };

        return factory.CreateConnection();
    }
}
