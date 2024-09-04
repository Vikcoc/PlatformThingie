using InventoryTemplateConsumer.dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace InventoryTemplateConsumer
{
    public class RInventoryTemplateConsumer(PInventoryTemplateRepo repo
        , [FromKeyedServices("InventoryTemplate")] IConnection connection) : BackgroundService
    {
        private readonly IConnection _connection = connection;
        private readonly PInventoryTemplateRepo _repo = repo;

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "templateRelease",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += HandleMessageEvent;
            channel.BasicConsume(queue: "templateRelease",
                     autoAck: true,
                     consumer: consumer,
                     consumerTag: "template");
            //block thread
            await Task.Run(stoppingToken.WaitHandle.WaitOne);

            channel.BasicCancel("template");
            channel.Close();
        }

        private void HandleMessageEvent(object? model, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var dto = JsonConvert.DeserializeObject<ReleaseTemplateDto>(Encoding.UTF8.GetString(body));
            if (_repo.ExistsTemplate(dto.TemplateName, dto.TemplateVersion).Result)
                _repo.DeleteAndRecreateParams(dto).Wait();
            else
                _repo.CreateTemplate(dto).Wait();
        }
    }
}
