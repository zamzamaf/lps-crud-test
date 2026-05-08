using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqDemo.Shared;

const string dlqArgument = "--dlq";

var settings = RabbitMqSettings.FromEnvironment();
var topology = new RabbitMqTopology(settings);

await using var connection = await topology.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// Reset existing topology to handle configuration changes
await topology.ResetTopologyAsync(channel);
await topology.EnsureTopologyAsync(channel);

var consumeDlq = args.Contains(dlqArgument, StringComparer.OrdinalIgnoreCase);
var queueName = consumeDlq ? settings.DeadLetterQueueName : settings.MainQueueName;

Console.WriteLine($"Consuming from queue '{queueName}' on '{settings.HostName}:{settings.Port}'.");
Console.WriteLine(consumeDlq ? "DLQ mode enabled: consuming dead-lettered messages." : "Normal mode: rejects route to DLQ when message body contains 'reject'.");

await channel.BasicQosAsync(0, 1, false);
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[>] Received: {message}");

    if (consumeDlq)
    {
        await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        Console.WriteLine("[v] Acknowledged DLQ message.");
        return;
    }

    if (message.Contains("reject", StringComparison.OrdinalIgnoreCase))
    {
        await channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false);
        Console.WriteLine("[!] Rejected message and sent to DLQ.");
    }
    else
    {
        await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        Console.WriteLine("[v] Acknowledged message.");
    }
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
Console.WriteLine("Consumer is running. Press Ctrl+C to exit.");

await WaitForExitAsync();

static Task WaitForExitAsync()
{
    var tcs = new TaskCompletionSource<object?>();
    Console.CancelKeyPress += (sender, eventArgs) =>
    {
        eventArgs.Cancel = true;
        tcs.TrySetResult(null);
    };
    return tcs.Task;
}
