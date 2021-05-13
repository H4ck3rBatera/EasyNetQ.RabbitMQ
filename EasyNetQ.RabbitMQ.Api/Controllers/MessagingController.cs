using EasyNetQ.RabbitMQ.Domain.Publish;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace EasyNetQ.RabbitMQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly IPub _pub;
        private readonly ILogger _logger;

        public MessagingController(IPub pub, ILogger<MessagingController> logger)
        {
            _pub = pub;
            _logger = logger;
        }

        [HttpPost]
        public async void Post(MessageAvailable message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(Post)}");

            try
            {
                await _pub.PublishAsync(message, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Message: {message.Text}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}