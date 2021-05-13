using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.RabbitMQ.Domain.Declare
{
    public interface IQueueDeclare
    {
        Task DeclareAsync(CancellationToken cancellationToken);
    }
}