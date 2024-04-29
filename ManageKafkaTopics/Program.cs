using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Add(new JsonConfigurationSource()
{
    Path = "conf/config.json"
});

var connection = builder.Configuration["Broker"];


var conf = new ConsumerConfig
{
    GroupId = "test-consumer-group",
    BootstrapServers = connection,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
{
    c.Subscribe("topic2");

    CancellationTokenSource cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        // Prevent the process from terminating.
        e.Cancel = true;
        cts.Cancel();
    };

    try
    {
        while (true)
        {
            try
            {
                var cr = c.Consume(cts.Token);
                Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occured: {e.Error.Reason}");
            }
        }
    }
    catch (OperationCanceledException)
    {
        // Ensure the consumer leaves the group cleanly and final offsets are committed.
        c.Close();
    }
}