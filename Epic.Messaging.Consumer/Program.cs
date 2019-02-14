using Epic.Rabbit.Subscriber;
using Epic.Rabbit.Subscriber.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Epic.Messaging.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            
            var builder = CreateWebHostBuilder(
                args.Where(arg => arg != "--console").ToArray());

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                builder.UseContentRoot(pathToContentRoot);
            }

            var host = builder.Build();

            if (isService)
            {
                // To run the app without the CustomWebHostService change the
                // next line to host.RunAsService();
                host.RunAsCustomService();
                
            }
            else
            {
                var options = host.Services.GetService(typeof(IOptions<RabbitSettings>)) as IOptions<RabbitSettings>;
                   
                ExecuteAsync("Epic.Request", options);

                Console.ReadLine();
            }
        }


        private static void ExecuteAsync(string queueName, IOptions<RabbitSettings> options)
        {
            
            var subscriber = new Subscriber(options);
            subscriber.Subscribe(queueName);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
