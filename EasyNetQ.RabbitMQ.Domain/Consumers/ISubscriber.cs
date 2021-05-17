using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.RabbitMQ.Domain.Consumers
{
    public interface ISubscriber
    {
        Task SubscribeAsync(Func<string, CancellationToken, Task> processMessageAsync,
            CancellationToken cancellationToken);
    }
}