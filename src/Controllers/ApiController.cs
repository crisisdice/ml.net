using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;

using Sentiment.Service;

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
        public IActionResult Get([Required] string data, [Required] string model)
        {
            try
            {
                var trainingModel = GetModel(model);

                var response = _service.Predict(data, trainingModel);

                return Ok(response);
            }
            catch (ArgumentException)
            {
                _logger.LogError("Model {model} not found", model);

                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError("Error while predicting {e}", e);
                return BadRequest();
            }

        }

        [HttpPost("data")]
        public IActionResult Post(DataRequest request)
        {
            try
            {
                _service.WriteTestData(request.Text, request.Score);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Error while saving data {e}", e);
                return BadRequest();
            }
        }

        [HttpPost("training")]
        public IActionResult Post(TrainingRequest request)
        {
            try
            {
                var model = GetModel(request.Model);

                _service.Train(model);

                return Ok();
            }
            catch (ArgumentException)
            {
                _logger.LogError("Model {model} not found", request.Model);

                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError("Error while training {e}", e);
                return BadRequest();
            }
        }

        public TrainingModel GetModel(string modelName) => (TrainingModel) Enum.Parse(typeof(TrainingModel), modelName);
    }

    public class DataRequest
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string Score { get; set; }
    }

    public class TrainingRequest
    {
        [Required]
        public string Model { get; set; }
    }
}
