using Epic.Messaging.Contracts;
using Epic.Messaging.Models.Person;
using Epic.Rabbit.Subscriber;
using Epic.Rabbit.Subscriber.Models;
using Epic.Rabbit.Subscriber.Processors;
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
                var messageProcessorFactory = host.Services.GetService(typeof(IMessageProcessorFactory)) as MessageProcessorFactory;
                var serializer = host.Services.GetService(typeof(Serializer<string>)) as Serializer<string>;

                ExecuteAsync("atlanta.request.person", options, messageProcessorFactory, serializer);

                Console.ReadLine();
            }
        }

        private static void ExecuteAsync(string queueName, IOptions<RabbitSettings> options, MessageProcessorFactory factory, Serializer<string> serializer)
        {
            var subscriber = new Subscriber(options, factory, serializer);
            //subscriber.Subscribe(queueName);
            subscriber.Subscribe(queueName, new PersonRequestData());

            Console.ReadLine();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
