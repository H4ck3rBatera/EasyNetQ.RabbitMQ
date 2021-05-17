using System;
using System.Threading;
using EasyNetQ.RabbitMQ.Domain.Consumers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNetQ.RabbitMQ.Domain.Support.Extensions
{
    public static class ServiceProviderExtension
    {
        public static void AddDomainProvider(this IServiceProvider serviceProvider)
        {
            var consumerService = serviceProvider.GetService<IConsumerService>();
            consumerService?.SubscribeAsync(new CancellationToken()).Wait();
        }
    }
}