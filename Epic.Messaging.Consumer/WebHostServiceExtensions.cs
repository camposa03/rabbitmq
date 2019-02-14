using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.ServiceProcess;

namespace Epic.Messaging.Consumer
{
    public static class WebHostServiceExtensions
    {
      
        public static void RunAsCustomService(this IWebHost host)
        {
            var subscriber = host.Services.GetService(typeof(ISubscriber)) as ISubscriber ?? null;
            var options = host.Services.GetService(typeof(RabbitSettings)) as RabbitSettings;
            var webHostService = new CustomWebHostService(host, subscriber, Options.Create(options));
            ServiceBase.Run(webHostService);
        }
    }
}
