using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.RabbitMQ.Domain.Providers
{
    public interface IQueueProvider
    {
        Task DeclareAsync(CancellationToken cancellationToken);
    }
}