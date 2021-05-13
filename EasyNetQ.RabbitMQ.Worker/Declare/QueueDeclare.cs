using EasyNetQ.RabbitMQ.Domain.Declare;
using EasyNetQ.RabbitMQ.Worker.Support.Options;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.RabbitMQ.Worker.Declare
{
    public class QueueDeclare : IQueueDeclare
    {
        private readonly IAdvancedBus _advancedBus;
        private readonly ILogger _logger;
        private readonly Exchanges _exchanges;
        private readonly Queues _queues;
        private readonly Routings _routings;

        public QueueDeclare(IBus bus, ILogger<QueueDeclare> logger,
            IOptions<Exchanges> exchanges, IOptions<Queues> queues, IOptions<Routings> routings)
        {
            _advancedBus = bus.Advanced;
            _logger = logger;
            _exchanges = exchanges.Value;
            _queues = queues.Value;
            _routings = routings.Value;
        }

        public async Task DeclareAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(DeclareAsync)}");

            try
            {
                var exchange = await _advancedBus.ExchangeDeclareAsync(name: _exchanges.ExchangeKey, type: ExchangeType.Direct, cancellationToken: cancellationToken);
                var queue = await _advancedBus.QueueDeclareAsync(name: _queues.QueueKey, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _advancedBus.BindAsync(exchange: exchange, queue: queue, routingKey: _routings.RoutingKey, headers: null, cancellationToken: cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Bind - Exchange: {_exchanges.ExchangeKey}, Queue: {_queues.QueueKey}, RoutingKey: {_routings.RoutingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}