using EasyNetQ.Logging;
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
                .Configure<Exchanges>(configuration.GetSection("Exchanges"))
                .Configure<Queues>(configuration.GetSection("Queues"));

            services.AddSingleton<IBus>((serviceProvider) =>
            {
                var connectionString = configuration.GetSection("ConnectionStrings:RabbitMQ");
                var virtualHost = configuration.GetSection("VirtualHosts:VirtualHostKey");

                var connectionConfiguration = new ConnectionConfiguration
                {
                    AmqpConnectionString = new Uri(connectionString.Value),
                    VirtualHost = virtualHost.Value
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

            return services;
        }
    }
}