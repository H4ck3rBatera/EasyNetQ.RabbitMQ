using EasyNetQ.RabbitMQ.Domain.Declare;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace EasyNetQ.RabbitMQ.Worker.Support.Extensions
{
    public static class ServiceProviderExtension
    {
        public static IServiceProvider AddWorkerProvider(this IServiceProvider serviceProvider)
        {
            var queueDeclare = serviceProvider.GetService<IQueueDeclare>();
            queueDeclare.DeclareAsync(new CancellationToken()).Wait();

            return serviceProvider;
        }
    }
}