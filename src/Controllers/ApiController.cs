using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;

using Sentiment.Logging;

namespace Sentiment.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly IPredictionService _service;

        public ApiController(ILogger<ApiController> logger, IPredictionService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IActionResult Get(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                _logger.LogError("No data provided");
                return BadRequest("No data provided");
            }

            var response = _service.Predict(data) ? "😀" : "☹️";

            _logger.LogInformation($"Predicted {data} as {response}");

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post(DataRequest request)
        {
            var text = request.Text;
            var score = request.Score;
            _logger.LogInformation($"Saving {text} as {score}");

            _service.WriteTestData(text, score);

            return Ok();
        }
    }

    public class DataRequest
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string Score { get; set; }
    }
}
