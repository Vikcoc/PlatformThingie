using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace UserPermissionConsumer
{
    public class RPermissionsConsumer([FromKeyedServices("UserPermission")]IConnection connection
        , [FromKeyedServices("UserPermission")] PUserPermissionRepo repo) : BackgroundService
    {
        private readonly IConnection _connection = connection;
        private readonly PUserPermissionRepo _repo = repo;

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "permissionRelease",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += HandleMessageEvent;
            channel.BasicConsume(queue: "permissionRelease",
                     autoAck: true,
                     consumer: consumer,
                     consumerTag: "permissions");
            //block thread
            await Task.Run(stoppingToken.WaitHandle.WaitOne);

            channel.BasicCancel("permissions");
            channel.Close();
        }

        private async void HandleMessageEvent(object? model, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var message = await Task.Run(() => JsonConvert.DeserializeObject<string[]>(Encoding.UTF8.GetString(body)));
            var toInsert = await _repo.OnlyMissing(message!);
            await _repo.AddPermissions(toInsert);
        }
    }
}
