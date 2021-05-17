using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.RabbitMQ.Domain.Publish
{
    public interface IPub
    {
        Task PublishAsync(MessageAvailable message,CancellationToken cancellationToken);
    }
}