using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber;
using Epic.Rabbit.Subscriber.Models;
using Epic.Rabbit.Subscriber.Settings;
using Epic.Serializers;
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
            //var isService = true;
            
            var builder = CreateWebHostBuilder(
                args.Where(arg => arg != "--console").ToArray());

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                builder.UseContentRoot(pathToContentRoot);
            }

            var host = builder.Build();

            var options = host.Services.GetService(typeof(IOptions<RabbitSettings>)) as IOptions<RabbitSettings>;

            if (isService)
            {
                // To run the app without the CustomWebHostService change the
                // next line to host.RunAsService();
                host.RunAsCustomService(options);
                
            }
            else
            {
                //var options = host.Services.GetService(typeof(IOptions<RabbitSettings>)) as IOptions<RabbitSettings>;
                var messageProcessor = host.Services.GetService(typeof(IMessageProcessor<TestMessage, string>)) as IMessageProcessor<TestMessage, string>;
                var serializer = host.Services.GetService(typeof(Serializer<string>)) as Serializer<string>;

                ExecuteAsync("Epic.Request", options, messageProcessor, serializer);

                Console.ReadLine();
            }
        }

        private static void ExecuteAsync(string queueName, IOptions<RabbitSettings> options, IMessageProcessor<TestMessage, string> processor, Serializer<string> serializer)
        {
            var subscriber = new Subscriber(options, processor, serializer);
            subscriber.Subscribe(queueName);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
