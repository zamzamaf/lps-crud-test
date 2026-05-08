using RabbitMQ.Client;

namespace RabbitMqDemo.Shared;

public sealed class RabbitMqTopology
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqTopology(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public ConnectionFactory CreateFactory()
    {
        return new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };
    }

    public async Task<IConnection> CreateConnectionAsync()
    {
        return await CreateFactory().CreateConnectionAsync();
    }

    public async Task ResetTopologyAsync(IChannel channel)
    {
        try
        {
            // Delete queues first (bindings will be removed automatically)
            await channel.QueueDeleteAsync(_settings.MainQueueName);
            await channel.QueueDeleteAsync(_settings.PendingQueueName);
            await channel.QueueDeleteAsync(_settings.DeadLetterQueueName);

            // Delete exchanges
            await channel.ExchangeDeleteAsync(_settings.MainExchangeName);
            await channel.ExchangeDeleteAsync(_settings.PendingExchangeName);
            await channel.ExchangeDeleteAsync(_settings.DeadLetterExchangeName);
        }
        catch (Exception)
        {
            // Ignore errors if queues/exchanges don't exist
        }
    }

    public async Task EnsureTopologyAsync(IChannel channel)
    {
        // Deklarasi Dead Letter Queue (final destination setelah retry gagal 10x)
        await channel.ExchangeDeclareAsync(_settings.DeadLetterExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
        await channel.QueueDeclareAsync(_settings.DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(_settings.DeadLetterQueueName, _settings.DeadLetterExchangeName, _settings.DeadLetterRoutingKey);

        // Deklarasi Pending Queue (intermediate queue untuk handle retry)
        var pendingQueueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = _settings.DeadLetterExchangeName,
            ["x-dead-letter-routing-key"] = _settings.DeadLetterRoutingKey,
            ["x-message-ttl"] = 5000 // TTL 5 detik untuk requeue
        };

        await channel.ExchangeDeclareAsync(_settings.PendingExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
        await channel.QueueDeclareAsync(_settings.PendingQueueName, durable: true, exclusive: false, autoDelete: false, arguments: pendingQueueArguments);
        await channel.QueueBindAsync(_settings.PendingQueueName, _settings.PendingExchangeName, _settings.PendingRoutingKey);

        // Deklarasi Main Queue dengan dead letter ke Pending Queue
        var mainQueueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = _settings.PendingExchangeName,
            ["x-dead-letter-routing-key"] = _settings.PendingRoutingKey
        };

        await channel.ExchangeDeclareAsync(_settings.MainExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
        await channel.QueueDeclareAsync(_settings.MainQueueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);
        await channel.QueueBindAsync(_settings.MainQueueName, _settings.MainExchangeName, _settings.MainRoutingKey);
    }
}
