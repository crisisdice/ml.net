using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;

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
