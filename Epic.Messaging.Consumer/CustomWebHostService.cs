﻿using Epic.Messaging.Contracts;
using Epic.Messaging.Models.Person;
using Epic.Rabbit.Subscriber.Models;
using Epic.Rabbit.Subscriber.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Epic.Messaging.Consumer
{
    internal class CustomWebHostService : WebHostService
    {
        private readonly ILogger _logger;
        private readonly ISubscriber _subscriber;
        private readonly RabbitSettings _options;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            
        }
        public CustomWebHostService(IWebHost host, ISubscriber subscriber, IOptions<RabbitSettings> options) : base(host)
        {
            _logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

  
        protected override void OnStarting(string[] args)
        {
            _logger.LogInformation("OnStarting method called.");
            base.OnStarting(args);

            try
            {
                File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", "About to start...");

            }
            catch (Exception e)
            {
                File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", e.ToString());
            }
        }
        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted method called.");
            File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", _options.Hostname ?? string.Empty);
            base.OnStarted();
            try
            {
        
                _subscriber.Subscribe(_options.QueueName, new PersonRequestData());
            }
            catch (Exception e)
            {
                File.AppendAllText("C:\\Temp\\rabbit_subscriber.txt", e.ToString());             
            }  
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping method called.");
            base.OnStopping();
        }

        internal void TestStartupAndStop(string[] args)
        {
            OnStarting(args);
            OnStarted();
            Console.ReadLine();
            OnStopping();
        }
    }
}
