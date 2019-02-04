using System;
using System.Collections.Generic;
using System.Text;

namespace Epic.Messaging.Contracts
{
    /// <summary>
    /// Contract to subscribe to a queue
    /// </summary>
    public interface ISubscriber
    {
        void Subscribe(string queueName);
    }
}
