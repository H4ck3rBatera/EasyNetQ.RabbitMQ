using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.RabbitMQ.Domain.Consumers.Services
{
    public interface IConsumerService
    {
        Task ProcessMessageAsync(string message, CancellationToken cancellationToken);
    }
}