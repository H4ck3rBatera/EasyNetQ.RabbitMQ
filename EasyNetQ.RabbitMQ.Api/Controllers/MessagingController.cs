using EasyNetQ.RabbitMQ.Domain.Publish;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Post(MessageAvailable message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Entering {nameof(Post)}");

            try
            {
                await _pub.PublishAsync(message, cancellationToken).ConfigureAwait(false);

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