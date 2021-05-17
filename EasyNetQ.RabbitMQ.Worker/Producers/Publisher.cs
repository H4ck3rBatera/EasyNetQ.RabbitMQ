using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Domain.Producers;
using EasyNetQ.RabbitMQ.Domain.Producers.Models;
using EasyNetQ.RabbitMQ.Worker.Support.Options;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyNetQ.RabbitMQ.Worker.Producers
{
    public class Publisher : IPublisher
    {
        private readonly IAdvancedBus _advancedBus;
        private readonly ILogger _logger;
        private readonly Exchanges _exchanges;
        private readonly Routings _routings;

        public Publisher(IBus bus, ILogger<Publisher> logger,
            IOptions<Exchanges> exchanges, IOptions<Routings> routings)
        {
            _advancedBus = bus.Advanced;
            _logger = logger;
            _exchanges = exchanges.Value;
            _routings = routings.Value;
        }

        public async Task PublishAsync(MessageModel messageAvailable, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(PublishAsync)}");

            try
            {
                var exchange = await _advancedBus.ExchangeDeclareAsync(name: _exchanges.ExchangeKey, type: ExchangeType.Direct, cancellationToken: cancellationToken).ConfigureAwait(false);

                var body = new Message<MessageModel>(messageAvailable);
                await _advancedBus.PublishAsync(exchange: exchange, routingKey: _routings.RoutingKey, mandatory: false, message: body, cancellationToken: cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Message: {messageAvailable.Text}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}