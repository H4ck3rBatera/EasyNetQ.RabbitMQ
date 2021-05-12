using Microsoft.AspNetCore.Mvc;

namespace EasyNetQ.RabbitMQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}