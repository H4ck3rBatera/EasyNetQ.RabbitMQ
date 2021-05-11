using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EasyNetQ.RabbitMQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}