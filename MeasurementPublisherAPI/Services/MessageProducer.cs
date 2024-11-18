using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace MeasurementPublisherAPI.Services
{
    public class MessageProducer : IMessageProducer
    {
        public async void SendMessage<T>(T message)
        {
            var factory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
            };
            var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "measurements",
                                 durable: true,
                                 exclusive: false, autoDelete: false, arguments: null);

            var jsonString = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(jsonString);

            await channel.BasicPublishAsync(exchange: "",
                                 routingKey: "measurements",
                                 body: body);
        }
    }
}
    