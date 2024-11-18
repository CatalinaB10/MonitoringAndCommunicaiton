using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace MeasurementPublisherAPI.Services
{
    public class MessageConsumer : IMessageConsumer
    {
        public async void ReadMessage<T>()
        {

            var factory = new ConnectionFactory()
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

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };

            await channel.BasicConsumeAsync(queue: "measurements",
                                 autoAck: true,
                                 consumer: consumer);
            Console.ReadKey();
        }
    }
}
