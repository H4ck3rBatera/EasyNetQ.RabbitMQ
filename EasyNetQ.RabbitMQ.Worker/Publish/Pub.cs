using EasyNetQ.RabbitMQ.Domain.Publish;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Worker.Support.Options;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using System.Text;

namespace EasyNetQ.RabbitMQ.Worker.Publish
{
    public class Pub : IPub
    {
        private readonly IAdvancedBus _advancedBus;
        private readonly ILogger _logger;
        private readonly Exchanges _exchanges;
        private readonly Routings _routings;

        public Pub(IBus bus, ILogger<Pub> logger,
            IOptions<Exchanges> exchanges, IOptions<Routings> routings)
        {
            _advancedBus = bus.Advanced;
            _logger = logger;
            _exchanges = exchanges.Value;
            _routings = routings.Value;
        }

        public async Task PublishAsync(MessageAvailable messageAvailable, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(PublishAsync)}");

            try
            {
                var exchange = await _advancedBus.ExchangeDeclareAsync(name: _exchanges.ExchangeKey, type: ExchangeType.Direct, cancellationToken: cancellationToken);

                var body = new Message<MessageAvailable>(messageAvailable);
                await _advancedBus.PublishAsync(exchange: exchange, routingKey: _routings.RoutingKey, mandatory: false, message: body, cancellationToken: cancellationToken);

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