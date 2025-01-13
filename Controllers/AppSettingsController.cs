using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoomBookingApi.Models;

namespace RoomBookingApi.Controllers
{

    [ApiController]
    [Route("/appsettings")]

    public class AppSettingsController : ControllerBase
    {
        private readonly ILogger<AppSettingsController> _logger;
        private readonly AppSettings _options;

        public AppSettingsController(ILogger<AppSettingsController> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult GetAppSettings()
        {
            _logger.LogInformation("Get application settings");
            return Ok(new {
                _options.ApiName,
                _options.Version
            });
        }
    }
}
