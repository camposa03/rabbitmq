using System;

namespace Epic.Rabbit.Subscriber.Models
{
    /// <summary>
    /// Models a test message
    /// </summary>
    public class TestMessage
    {
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid Id { get; set; }

    }
}
