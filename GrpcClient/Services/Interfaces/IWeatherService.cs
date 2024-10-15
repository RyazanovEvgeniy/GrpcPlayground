using GrpcContracts;

namespace GrpcClient.Services.Interfaces;

public interface IWeatherService
{
    Task<WeatherResponse> GetCurrentWeather(string city);
}