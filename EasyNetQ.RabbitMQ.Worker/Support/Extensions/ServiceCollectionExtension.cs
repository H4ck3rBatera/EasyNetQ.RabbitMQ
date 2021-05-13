using EasyNetQ.Logging;
using EasyNetQ.RabbitMQ.Domain.Declare;
using EasyNetQ.RabbitMQ.Worker.Declare;
using EasyNetQ.RabbitMQ.Worker.Support.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EasyNetQ.RabbitMQ.Worker.Support.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration configuration)
        {
            LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);

            services
                .Configure<Exchanges>(configuration.GetSection("RabbitMQ:Exchanges"))
                .Configure<Queues>(configuration.GetSection("RabbitMQ:Queues"))
                .Configure<Routings>(configuration.GetSection("RabbitMQ:Routings"));

            services.AddSingleton<IBus>((serviceProvider) =>
            {
                var connectionString = configuration.GetSection("RabbitMQ:ConnectionStrings:RabbitMQKey");
                
                var connectionConfiguration = new ConnectionConfiguration
                {
                    AmqpConnectionString = new Uri(connectionString.Value),
                };

                return RabbitHutch.CreateBus(connectionConfiguration, serviceRegister =>
                {
                    serviceRegister.Register<ISerializer>(_ => new JsonSerializer(new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                });
            });

            services
                .AddScoped<IQueueDeclare, QueueDeclare>();

            return services;
        }
    }
}