using Connect.Api.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
      
        private readonly ILogger<WeatherForecastController> _logger;

        public IWeatherService WeatherService { get; }

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IWeatherService weatherService)
        {
            _logger = logger;
            WeatherService = weatherService;
        }

        //public Action<string> MyProperty = (s) => { };
        //public Func<string, string ,string,int> MyProperty2 = (s,s1,s2) => 2;


        [Authorize]
        [HttpGet("GetWeatherForecast")]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            var x = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = "Summary"
            });

            return x;
        }

        [AllowAnonymous]
        [HttpPost("GetDetails")]
        public async Task<IActionResult> GetDetails()
        {
            var result = await WeatherService.GetWeatherForecasts<IEnumerable<WeatherForecast>>();
            return Ok();
        }
    }
}