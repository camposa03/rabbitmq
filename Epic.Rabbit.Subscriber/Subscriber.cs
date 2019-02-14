using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Epic.Rabbit.Subscriber
{
    public class Subscriber : ISubscriber
    {
        private readonly RabbitSettings _options;
        public Subscriber(IOptions<RabbitSettings> options)
        {
            _options = options.Value;
        }
        public void ReceiveMessage(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = _options.Hostname };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
        
                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Debug.WriteLine(" [x] {0}", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public void Subscribe(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = _options.Hostname, DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", "Created channel");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    await Task.Delay(2000);
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Message Received: {message}");

                    File.WriteAllText("C:\\Temp\\rabbit_subscriber.txt", message);
                    
                    channel.BasicAck(ea.DeliveryTag, false);

                    Console.WriteLine($"Acknowledged message with DeliveryTag: {ea.DeliveryTag}");

                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
        }
    }
}
