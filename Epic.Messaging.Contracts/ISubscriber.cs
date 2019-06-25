namespace Epic.Messaging.Contracts
{
    /// <summary>
    /// Contract to subscribe to a queue
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Subscribes to a queue
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="queueName">The queue you'd like to subscribe to</param>
        /// <param name="message">The type of message contained in the queue</param>
        void Subscribe<TMessage>(string queueName, TMessage message) where TMessage : class;
    }
}
