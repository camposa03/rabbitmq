using Epic.Messaging.Contracts;
using Microsoft.AspNetCore.Hosting;
using System.ServiceProcess;

namespace Epic.Messaging.Consumer
{
    public static class WebHostServiceExtensions
    {
      
        public static void RunAsCustomService(this IWebHost host)
        {
            var subscriber = host.Services.GetService(typeof(ISubscriber)) as ISubscriber ?? null;
            var webHostService = new CustomWebHostService(host, subscriber);
            ServiceBase.Run(webHostService);
        }
    }
}
