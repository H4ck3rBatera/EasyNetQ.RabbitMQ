using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Domain.Producers.Models;

namespace EasyNetQ.RabbitMQ.Domain.Producers
{
    public interface IPublisher
    {
        Task PublishAsync(MessageModel message,CancellationToken cancellationToken);
    }
}