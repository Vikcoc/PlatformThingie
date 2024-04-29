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



var tcs = new TaskCompletionSource<bool>();
async Task TimerToOk()
{
    for(int i = 10; i > 0; i--)
    {
        Console.WriteLine("Going in {0} seonds", i);
        await Task.Delay(1000);
    }
    tcs.TrySetResult(true);
}

var config = new AdminClientConfig { 
    BootstrapServers = connection,
};

//because it runs on a background thread
//because fuck me i guess
Console.WriteLine("Before build");
var adminClient = new AdminClientBuilder(config)
    .SetLogHandler((a, b) => {
        //I hope that it will always have error as the first log if error
        Console.WriteLine(b.Message);
        tcs.TrySetResult(b.Level != SyslogLevel.Error);
        })
    .Build();
var tsk = TimerToOk();
Console.WriteLine("After build");
var res = await tcs.Task;
Console.WriteLine("After await");
if (!res)
    throw new Exception("Could not work");
Console.WriteLine("After error check");
var existingTopics = adminClient.GetMetadata(new TimeSpan(0, 1, 0)).Topics.Select(x => x.Topic);
Console.WriteLine(existingTopics == null);
foreach (var x in existingTopics!)
    Console.WriteLine(x);
var newTopics = topics!.Except(existingTopics)
    .Select(x => new TopicSpecification
    {
        Name = x
    });
foreach (var x in newTopics)
    Console.WriteLine(x.Name);
await adminClient.CreateTopicsAsync(newTopics, new CreateTopicsOptions
{
    RequestTimeout = TimeSpan.FromSeconds(2)
});