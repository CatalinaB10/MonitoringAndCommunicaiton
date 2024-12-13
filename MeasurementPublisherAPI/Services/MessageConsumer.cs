using MeasurementPublisherAPI.Context;
using MeasurementPublisherAPI.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using static System.Collections.Specialized.BitVector32;
using System.Text.RegularExpressions;

namespace MeasurementPublisherAPI.Services
{
    public class MessageConsumer(IServiceProvider serviceProvider) : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private IModel _channel1;
        private IConnection _connection;

        private readonly string _queueName1 = "MeasurementQueue";

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqp://guest:guest@rabbit-server:5672");
            factory.ClientProvidedName = "Measurement Receiver";
            _connection = factory.CreateConnection();
            _channel1 = _connection.CreateModel();
          
            //_channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);


            _channel1.QueueDeclare("MeasurementQueue", false, false, false, null);

            var consumer1 = new EventingBasicConsumer(_channel1);
            consumer1.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received measurement: {message}");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MeasurementContext>();

                await ProcessMeasurementAsync(message, context);

                // Acknowledge the message
                _channel1.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel1.BasicConsume(queue: "MeasurementQueue", autoAck: false, consumer: consumer1);

            return Task.CompletedTask;
        }
        private async Task ProcessMeasurementAsync(string message, MeasurementContext context)
        {
            // Deserialize the message and store it in the database
            Measurement measurement = new Measurement();
            measurement.Value = Convert.ToDouble(message);
            measurement.Timestamp = DateTime.UtcNow;
            measurement.DeviceId = Guid.Parse("daf5d2ae-d5ac-4c8d-a019-d0c95091fc04");

            if (measurement != null)
            {
                context.Measurements.Add(measurement);
                await context.SaveChangesAsync();
            }
        }


        public override void Dispose()
        {
            _channel1?.Close();
            _connection?.Close();
            base.Dispose();
        }

    }
}
