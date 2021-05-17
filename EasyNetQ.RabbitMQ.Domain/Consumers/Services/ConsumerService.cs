using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Domain.Producers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyNetQ.RabbitMQ.Domain.Consumers.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger _logger;

        public ConsumerService(ISubscriber subscriber, ILogger<ConsumerService> logger)
        {
            _subscriber = subscriber;
            _logger = logger;
        }

        public async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            await _subscriber.SubscribeAsync(ProcessMessageAsync, cancellationToken);
        }

        private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(ProcessMessageAsync)}");

            await Task.Factory.StartNew(() =>
            {
                var messageModel = JsonConvert.DeserializeObject<MessageModel>(message);

                _logger.LogInformation($"Message: {messageModel?.Text}");
            }, cancellationToken);
        }
    }
}