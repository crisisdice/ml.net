using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sentiment.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly IPredictionServices _service;

        public ApiController(ILogger<ApiController> logger, IPredictionServices service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IActionResult Get(string data)
        {
            if (string.IsNullOrEmpty(data))
                return BadRequest("No data");

            var response = _service.Predict(data) ? "😀" : "☹️";

            _logger.LogInformation("Predicted {data} as {response}");

            return Ok(response);
        }
    }
}
