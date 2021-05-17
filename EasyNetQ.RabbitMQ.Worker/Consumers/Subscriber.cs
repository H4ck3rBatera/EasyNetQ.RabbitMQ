using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Domain.Consumers;
using EasyNetQ.RabbitMQ.Worker.Support.Options;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyNetQ.RabbitMQ.Worker.Consumers
{
    public class Subscriber : ISubscriber
    {
        private readonly IAdvancedBus _advancedBus;
        private readonly ILogger _logger;
        private readonly Exchanges _exchanges;
        private readonly Queues _queues;
        private readonly Routings _routings;

        public Subscriber(IBus bus, ILogger<Subscriber> logger,
            IOptions<Exchanges> exchanges, IOptions<Queues> queues, IOptions<Routings> routings)
        {
            _advancedBus = bus.Advanced;
            _logger = logger;
            _exchanges = exchanges.Value;
            _queues = queues.Value;
            _routings = routings.Value;
        }

        public async Task SubscribeAsync(Func<string, CancellationToken, Task> processMessageAsync, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(SubscribeAsync)}");

            try
            {
                var exchange = await _advancedBus.ExchangeDeclareAsync(name: _exchanges.ExchangeKey, type: ExchangeType.Direct, cancellationToken: cancellationToken).ConfigureAwait(false);
                var queue = await _advancedBus.QueueDeclareAsync(name: _queues.QueueKey, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken).ConfigureAwait(false);
                await _advancedBus.BindAsync(exchange: exchange, queue: queue, routingKey: _routings.RoutingKey, headers: null, cancellationToken: cancellationToken).ConfigureAwait(false);

                _advancedBus.Consume(queue, (body, _, _) => Task.Factory.StartNew(async () =>
                {
                    var message = Encoding.UTF8.GetString(body);
                    await processMessageAsync(message, cancellationToken).ConfigureAwait(false);
                }, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}