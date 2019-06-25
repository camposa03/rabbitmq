using Epic.Backend.Models;
using Epic.Messaging.Contracts;
using Epic.Messaging.Models.Person;
using System.Threading.Tasks;

namespace Epic.Rabbit.Subscriber.Processors
{
    /// <summary>
    /// Subscribes and processes person target messages
    /// </summary>
    public class PersonTargetMessageProcessor : IMessageProcessor<PersonRequestData, string>
    {
        public async Task<ContentResponseModel<string>> ProcessAsync(PersonRequestData message)
        {
            await Task.Delay(1000);
            var response = new ContentResponseModel<string>()
            {
                Successful = true,
                Message = "Successfully processed person Target request",
                TransactionReceipt = message.Receipt.ToString(),
                Content = "mock person target response"
            };

            return response;
        }
    }
}
