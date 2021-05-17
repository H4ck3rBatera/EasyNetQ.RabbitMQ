using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using EasyNetQ.RabbitMQ.Domain.Providers;

namespace EasyNetQ.RabbitMQ.Worker.Support.Extensions
{
    public static class ServiceProviderExtension
    {
        public static void AddWorkerProvider(this IServiceProvider serviceProvider)
        {
            var queueProvider = serviceProvider.GetService<IQueueProvider>();
            queueProvider?.DeclareAsync(new CancellationToken()).Wait();
        }
    }
}