using Epic.Messaging.Contracts;
using Epic.Messaging.Models.Person;
using Epic.Rabbit.Subscriber.Models;

namespace Epic.Rabbit.Subscriber.Processors
{
    public class MessageProcessorFactory : IMessageProcessorFactory
    {
        public IMessageProcessor<TMessage, TContent> Create<TMessage, TContent>(TMessage message)
        {
            IMessageProcessor<TMessage, TContent> messageProcessor = null;
            switch (message)
            {
                case TestMessage testMessage:
                    messageProcessor = new MockMessageProcessor() as IMessageProcessor<TMessage, TContent>;
                    break;
                case PersonRequestData personMessage:
                    messageProcessor = new PersonTargetMessageProcessor() as IMessageProcessor<TMessage, TContent>;
                    break;
                default:
                    break;
            }

            return messageProcessor;
        }

        
    }
}
