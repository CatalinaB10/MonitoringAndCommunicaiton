using MeasurementPublisherAPI.Context;
using MeasurementPublisherAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MeasurementPublisherAPI.Services;

public class DeviceConsumer(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private IModel _channel1, _channel2;
    private IConnection _connection;


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConnectionFactory factory = new();
        factory.Uri = new Uri("amqp://guest:guest@rabbit-server:5672");
        factory.ClientProvidedName = "Device Receiver";
        _connection = factory.CreateConnection();

        _channel2 = _connection.CreateModel();

        //_channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        //_channel2.ExchangeDeclare("device_topic", ExchangeType.Direct);
        _channel2.QueueDeclare("DeviceTopicQueue", false, false, false, null);
        //_channel2.QueueBind("DeviceTopicQueue", "device_topic", "device.*");
        //_channel2.BasicQos(0, 1, false);

        

        var consumer2 = new EventingBasicConsumer(_channel2);
        consumer2.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received device with details: {message}");

            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MeasurementContext>();

            var action = ea.RoutingKey;
            await ProcessDeviceAsync(message, action, context);

            // Acknowledge the message
            _channel2.BasicAck(ea.DeliveryTag, multiple: false);
        };

         _channel2.BasicConsume(queue: "DeviceTopicQueue", autoAck: false, consumer: consumer2);

        return Task.CompletedTask;
    }
    private async Task ProcessDeviceAsync(string message, string action, MeasurementContext context)
    {
        //_ = new Device();
        Device device = JsonConvert.DeserializeObject<Device>(message);
        var existing = await context.Devices.FindAsync(device.Id);
        if(existing == null)
        {
            context.Devices.Add(device);
        }
        else
        {
            context.Devices.Update(device);
        }
        await context.SaveChangesAsync();
        //if (device != null)
        //{
        //    switch (action)
        //    {
        //        case "device.delete":
        //            if(existing != null)
        //            {
        //                context.Devices.Remove(device);
        //            }
        //            break;
        //        case "device.update":
        //            if (existing != null)
        //            {
        //                context.Devices.Update(device);
        //            }
        //            break;
        //        case "device.create":
        //            context.Devices.Add(device);
        //            break;
        //        default:
        //            break;
        //    }
        //await context.SaveChangesAsync();
    }
    


    public override void Dispose()
    {
        _channel2?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
