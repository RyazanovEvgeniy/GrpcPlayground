using Microsoft.AspNetCore.Mvc;

using GrpcClient.Services.Interfaces;
using GrpcContracts;

namespace GrpcClient.Controllers;

[Route("api/test")]
[ApiController]
public class ControllerTest(ILogger<ControllerTest> logger, IWeatherService weatherService) : ControllerBase
{
    private readonly ILogger<ControllerTest> _logger = logger;
    private readonly IWeatherService _weatherService = weatherService;

    [HttpGet]
    public async Task<ActionResult<List<WeatherResponse>>> Get(string city)
    {
        _logger.LogInformation("Requestiing weather for city {City}.", city);
        var response = await _weatherService.GetCurrentWeather(city);
        return Ok(response);
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}