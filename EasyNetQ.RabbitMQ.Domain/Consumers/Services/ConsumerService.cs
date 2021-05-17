using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Domain.Producers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyNetQ.RabbitMQ.Domain.Consumers.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ILogger _logger;

        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(ProcessMessageAsync)}");

            await Task.Factory.StartNew(() =>
            {
                var messageModel = JsonConvert.DeserializeObject<MessageModel>(message);

                _logger.LogInformation($"Message: {messageModel}");
            }, cancellationToken);
        }
    }
}