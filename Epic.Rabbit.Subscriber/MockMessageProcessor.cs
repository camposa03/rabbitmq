using Epic.Backend.Models;
using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber.Models;
using System.Threading.Tasks;

namespace Epic.Rabbit.Subscriber
{
    /// <summary>
    /// Mock implementation of <see cref="IMessageProcessor{TMessage, TContent}"/>
    /// </summary>
    public class MockMessageProcessor : IMessageProcessor<TestMessage, string>
    {
        public async Task<ContentResponseModel<string>> ProcessAsync(TestMessage message)
        {
            await Task.Delay(2000);
            var response = new ContentResponseModel<string>()
            {
                Successful = true,
                Message = "Successfully processed your message",
                TransactionReceipt = message.Id.ToString(),
                Content = "mock response"
            };

            return response;
        }
    }
}
