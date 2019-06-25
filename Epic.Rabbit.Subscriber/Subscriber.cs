using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber.Settings;
using Epic.Serializers;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;

namespace Epic.Rabbit.Subscriber
{
    /// <summary>
    /// Subscribes to messages from RabbitMQ
    /// </summary>
    public class Subscriber : ISubscriber
    {  
        private readonly RabbitSettings _options;
        private readonly IMessageProcessorFactory _factory;
        private readonly Serializer<string> _serializer;

        public Subscriber(IOptions<RabbitSettings> options, IMessageProcessorFactory factory, Serializer<string> serializer)
        {
            _options = options.Value;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        #region ISubscriber Members
        public void Subscribe<TMessage>(string queueName, TMessage messageType) where TMessage : class
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

                    TMessage deserializedMessage = _serializer.Deserialize(message, messageType);

                    var messageProcessor = _factory.Create<TMessage, string>(deserializedMessage);
                    var messageProcessorResponse = await messageProcessor.ProcessAsync(deserializedMessage);
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

        #endregion
        public void SubscribeTest<TMessage>(string queueName, TMessage messageType) where TMessage : class
        {

            var factory = new ConnectionFactory() { HostName = _options.Hostname, VirtualHost = _options.VirtualHost};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    TMessage testMessage = _serializer.Deserialize(message, messageType);

                    var messageProcessor = _factory.Create<TMessage, string>(testMessage);
                    var messageProcessorResponse = await messageProcessor.ProcessAsync(testMessage);
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

        private byte[] ToBytes(string message) => Encoding.UTF8.GetBytes(message);

    }
}
