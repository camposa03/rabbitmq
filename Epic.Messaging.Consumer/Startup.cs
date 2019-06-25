using Epic.Messaging.Contracts;
using Epic.Rabbit.Subscriber;
using Epic.Rabbit.Subscriber.Models;
using Epic.Rabbit.Subscriber.Processors;
using Epic.Rabbit.Subscriber.Settings;
using Epic.Serializers;
using Epic.Serializers.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Epic.Messaging.Consumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RabbitSettings>(Configuration.GetSection("RabbitSettings"));

            
            services.AddSingleton<ISubscriber, Subscriber>();
            services.AddSingleton<Serializer<string>, JsonSerializer>();
            //Plug in your specific implementation
            services.AddSingleton<IMessageProcessor<TestMessage, string>, MockMessageProcessor>();
            services.AddSingleton<IMessageProcessorFactory, MessageProcessorFactory>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
