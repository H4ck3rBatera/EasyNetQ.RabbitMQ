using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.RabbitMQ.Domain.Producers;
using EasyNetQ.RabbitMQ.Domain.Producers.Models;

namespace EasyNetQ.RabbitMQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly IPublisher _publisher;
        private readonly ILogger _logger;

        public ProducerController(IPublisher publisher, ILogger<ProducerController> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(MessageModel message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(Post)}");

            try
            {
                await _publisher.PublishAsync(message, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Message: {message.Text}");

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}