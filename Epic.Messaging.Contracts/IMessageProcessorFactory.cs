namespace Epic.Messaging.Contracts
{
    /// <summary>
    /// Contract for obtaining a <see cref="IMessageProcessor{TMessage, TContent}"/>
    /// </summary>
    public interface IMessageProcessorFactory
    {
        /// <summary>
        /// Creates a <see cref="IMessageProcessor{TMessage, TContent}"/>
        /// </summary>
        /// <typeparam name="TMessage">The type of message contained in the queue</typeparam>
        /// <typeparam name="TContent">The type of message contained in the response from processing a message"/></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        IMessageProcessor<TMessage, TContent> Create<TMessage,TContent>(TMessage message);
    }
}
