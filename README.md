# EasyNetQ.RabbitMQ
- EasyNetQ with RabbitMQ
- Docker
- .NET 5.0
- Patterns
	- Worker Pattern
	- Option Pattern
	- Provider Pattern
- Swashbuckle.AspNetCore
- Dependency Injection

### Extensions

```csharp
public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration configuration)
        {
            LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);

            services
                .Configure<Exchanges>(configuration.GetSection("RabbitMQ:Exchanges"))
                .Configure<Queues>(configuration.GetSection("RabbitMQ:Queues"))
                .Configure<Routings>(configuration.GetSection("RabbitMQ:Routings"));

            services.AddSingleton<IBus>((serviceProvider) =>
            {
                var connectionString = configuration.GetSection("RabbitMQ:ConnectionStrings:RabbitMQKey");

                var connectionConfiguration = new ConnectionConfiguration
                {
                    AmqpConnectionString = new Uri(connectionString.Value),
                };

                return RabbitHutch.CreateBus(connectionConfiguration, serviceRegister =>
                {
                    serviceRegister.Register<ISerializer>(_ => new JsonSerializer(new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                });
            });

            services
                .AddScoped<IQueueProvider, QueueProvider>()
                .AddScoped<ISubscriber, Subscriber>()
                .AddScoped<IPublisher, Publisher>();

            return services;
        }
    }
```

### Queue Provider

```csharp
public class QueueProvider : IQueueProvider
    {
        private readonly IAdvancedBus _advancedBus;
        private readonly ILogger _logger;
        private readonly Exchanges _exchanges;
        private readonly Queues _queues;
        private readonly Routings _routings;

        public QueueProvider(IBus bus, ILogger<QueueProvider> logger,
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
```

### Producers

```csharp
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
                var exchange = await _advancedBus.ExchangeDeclareAsync(name: _exchanges.ExchangeKey, type: ExchangeType.Direct, cancellationToken: cancellationToken);

                var body = new Message<MessageModel>(messageAvailable);
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
```

### Consumers

```csharp
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
                var exchange = await _advancedBus.ExchangeDeclareAsync(name: _exchanges.ExchangeKey, type: ExchangeType.Direct, cancellationToken: cancellationToken);
                var queue = await _advancedBus.QueueDeclareAsync(name: _queues.QueueKey, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _advancedBus.BindAsync(exchange: exchange, queue: queue, routingKey: _routings.RoutingKey, headers: null, cancellationToken: cancellationToken).ConfigureAwait(false);

                _advancedBus.Consume(queue, (body, _, _) => Task.Factory.StartNew(async () =>
                {
                    var message = Encoding.UTF8.GetString(body);
                    await processMessageAsync(message, cancellationToken);
                }, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
```
