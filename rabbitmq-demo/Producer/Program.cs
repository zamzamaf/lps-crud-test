using System.Text;
using RabbitMQ.Client;
using RabbitMqDemo.Shared;

const int defaultMessageCount = 10;

var settings = RabbitMqSettings.FromEnvironment();
var topology = new RabbitMqTopology(settings);

await using var connection = await topology.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// Reset existing topology to handle configuration changes
await topology.ResetTopologyAsync(channel);
await topology.EnsureTopologyAsync(channel);

var messageCount = args.Length > 0 && int.TryParse(args[0], out var parsed) && parsed > 0
    ? parsed
    : defaultMessageCount;

Console.WriteLine($"Publishing {messageCount} messages to queue '{settings.MainQueueName}' on '{settings.HostName}:{settings.Port}'.");

for (var index = 1; index <= messageCount; index++)
{
    var bodyText = index % 5 == 0 ? $"Message {index} - reject" : $"Message {index}";
    var body = Encoding.UTF8.GetBytes(bodyText);

    var properties = new BasicProperties { Persistent = true };

    await channel.BasicPublishAsync(
        exchange: settings.MainExchangeName,
        routingKey: settings.MainRoutingKey,
        mandatory: false,
        basicProperties: properties,
        body: body);

    Console.WriteLine($"[x] Published: {bodyText}");
}

Console.WriteLine("All messages published. Press any key to exit.");
Console.ReadKey();
