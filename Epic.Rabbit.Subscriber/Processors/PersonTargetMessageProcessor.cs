using Epic.Backend.Models;
using Epic.Messaging.Contracts;
using Epic.Messaging.Models.Person;
using System;
using System.Threading.Tasks;

namespace Epic.Rabbit.Subscriber.Processors
{
    /// <summary>
    /// Subscribes and processes person target messages
    /// </summary>
    public class PersonTargetMessageProcessor : IMessageProcessor<PersonRequestData, string>
    {
        public Task<ContentResponseModel<string>> ProcessAsync(PersonRequestData message)
        {
            throw new NotImplementedException();
        }
    }
}
