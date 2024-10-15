using GrpcContracts;
using GrpcClient.Services.Interfaces;

namespace GrpcClient.Services;

public class WeatherService(ILogger<WeatherService> logger, Weather.WeatherClient client) : IWeatherService
{
    private readonly ILogger<WeatherService> _logger = logger;
    private readonly Weather.WeatherClient _client = client;

    public async Task<WeatherResponse> GetCurrentWeather(string city) =>
        await _client.GetCurrentWeatherAsync(new GerCurrentWeatherRequest{ City = city });
}