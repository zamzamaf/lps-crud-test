namespace RabbitMqDemo.Shared;

public sealed class RabbitMqSettings
{
    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";

    public string MainExchangeName { get; init; } = "app.exchange";
    public string MainQueueName { get; init; } = "app.queue";
    public string MainRoutingKey { get; init; } = "app.key";

    public string DeadLetterExchangeName { get; init; } = "app.dlx.exchange";
    public string DeadLetterQueueName { get; init; } = "app.dlq.queue";
    public string DeadLetterRoutingKey { get; init; } = "app.dlq.key";

    public string PendingExchangeName { get; init; } = "app.dlq.exchange.pending";
    public string PendingQueueName { get; init; } = "app.dlq.pending";
    public string PendingRoutingKey { get; init; } = "app.dlq.key.pending";
    public int MaxRetryAttempts { get; init; } = 10;

    public static RabbitMqSettings FromEnvironment()
    {
        return new RabbitMqSettings
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var port) ? port : 5672,
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
            VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? "/"
        };
    }
}
