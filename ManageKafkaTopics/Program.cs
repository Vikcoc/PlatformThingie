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
var topics = builder.Configuration.GetSection("Topics").Get<string[]>();

var config = new AdminClientConfig { 
    BootstrapServers = connection,
};

var tcs = new TaskCompletionSource<bool>();
//because it runs on a background thread
//because fuck me i guess
var adminClient = new AdminClientBuilder(config)
    .SetLogHandler((a, b) => {
        //I hope that it will always have error as the first log if error
        Console.WriteLine(b.Message);
        tcs.TrySetResult(b.Level != SyslogLevel.Error);
        })
    .Build();

var res = await tcs.Task;

if (!res)
    throw new Exception("Could not work");
    
var newTopics = topics!.Except(adminClient.GetMetadata(new TimeSpan(0, 1, 0)).Topics.Select(x => x.Topic))
    .Select(x => new TopicSpecification
    {
        Name = x,
        NumPartitions = 1,
        ReplicationFactor = 1
    });

await adminClient.CreateTopicsAsync(newTopics);