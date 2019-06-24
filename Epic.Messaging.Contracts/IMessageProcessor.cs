using Epic.Backend.Models;
using System.Threading.Tasks;

namespace Epic.Messaging.Contracts
{
    /// <summary>
    /// Contract to process an incoming message
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TContent"></typeparam>
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
