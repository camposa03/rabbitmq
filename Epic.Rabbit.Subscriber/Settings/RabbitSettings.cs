namespace Epic.Rabbit.Subscriber.Settings
{
    /// <summary>
    /// Container class to hold connection parameters to the
    /// RabbitMQ server
    /// </summary>
    public class RabbitSettings
    {
        /// <summary>
        /// The hostname of the RabbitMQ server
        /// </summary>
        public string Hostname { get; set; } = "localhost";

        /// <summary>
        /// The username used to connect to RabbitMQ server
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// The password used to connect to RabbitMQ server
        /// </summary>
        public string Password { get; set; } = "guest";

        public string QueueName { get; set; } = "Epic.Request";
    }
}
