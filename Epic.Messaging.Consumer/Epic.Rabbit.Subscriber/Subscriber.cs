using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Epic.Rabbit.Subscriber
{
    public class Subscriber
    {
        public void ReceiveMessage(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
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

        public void ReceiveMessageAsync(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", "Created channel");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    await Task.Delay(2000);
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);                
            }
        }

    
    }
}
