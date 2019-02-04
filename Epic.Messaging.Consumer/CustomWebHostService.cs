﻿using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Epic.Messaging.Consumer
{
    internal class CustomWebHostService : WebHostService
    {
        private readonly ILogger _logger;
        private readonly ISubscriber _subscriber;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            
        }
        public CustomWebHostService(IWebHost host, ISubscriber subscriber) : base(host)
        {
            _logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber)); 
            
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
            base.OnStarted();
            try
            {
                var subscriber = new Subscriber();
                _subscriber.Subscribe("test");
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
