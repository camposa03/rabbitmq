using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber.Models;
using Epic.Rabbit.Subscriber.Settings;
using Epic.Serializers;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Epic.Rabbit.Subscriber
{
    /// <summary>
    /// Subscribes to messages from RabbitMQ
    /// </summary>
    public class Subscriber : ISubscriber
    {  
        private readonly RabbitSettings _options;
        private readonly IMessageProcessor<TestMessage, string> _processor;
        private readonly Serializer<string> _serializer;

        public Subscriber(IOptions<RabbitSettings> options, IMessageProcessor<TestMessage, string> processor, Serializer<string> serializer)
        {
            _options = options.Value;
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void ReceiveMessage(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = _options.Hostname, DispatchConsumersAsync = true };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
        
                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var testMessage = _serializer.Deserialize(message, new TestMessage());

                    var messageProcessorResponse = await _processor.ProcessAsync(testMessage);
                    var jsonMessage = _serializer.Serialize(messageProcessorResponse);

                    if (messageProcessorResponse.Successful)
                    {
                        //Reply to originator
                        var properties = ea.BasicProperties;
                        var replyTo = properties.ReplyTo;
                        channel.BasicPublish(exchange: string.Empty,
                                             routingKey: replyTo,
                                             basicProperties: null,
                                             body: ToBytes(jsonMessage));
                        Debug.WriteLine(" [x] {0}", message);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        Debug.WriteLine("Error trying to process your message. Will re-queue");
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                   
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
            Debug.WriteLine("Connection created...");

            
            var channel = connection.CreateModel();
            
            channel.QueueDeclare(queue: queueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", "Created channel");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                   
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                TestMessage testMessage;
                try
                {
                    testMessage = _serializer.Deserialize(message, new TestMessage());
                }
                catch (Exception ex)
                {

                    throw;
                }
               
                var messageProcessor = await _processor.ProcessAsync(testMessage);

                if (messageProcessor.Successful)
                {
                    Debug.WriteLine(" [x] {0}", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    Debug.WriteLine("Error trying to process your message. Will send to error queue");
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }

                Console.WriteLine($"Message Received: {message}");

                File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", message);
                    
                Console.WriteLine($"Acknowledged message with DeliveryTag: {ea.DeliveryTag}");

            };

            channel.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);
            
        }
        private byte[] ToBytes(string message) => Encoding.UTF8.GetBytes(message);

    }
}
