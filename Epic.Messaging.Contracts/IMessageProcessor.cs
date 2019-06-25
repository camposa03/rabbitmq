using Epic.Backend.Models;
using System.Threading.Tasks;

namespace Epic.Messaging.Contracts
{
    /// <summary>
    /// Contract to process messages from a queue
    /// </summary>
    /// <typeparam name="TMessage">The type of message contained in a queue</typeparam>
    /// <typeparam name="TContent">The type of message contained in the response from processing a message</typeparam>
    public interface IMessageProcessor<TMessage, TContent>
    {
        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ContentResponseModel<TContent>> ProcessAsync(TMessage message);
    }
}
