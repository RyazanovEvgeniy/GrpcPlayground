using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcPlayground.Services;

public class WeatherService(ILogger<WeatherService> logger) : Weather.WeatherBase
{
    private readonly ILogger<WeatherService> _logger = logger;

    public override Task<WeatherResponse> GetCurrentWeather(GerCurrentWeatherRequest request, ServerCallContext context)
    {
        return Task.FromResult(new WeatherResponse
        {
            Temperature = new Random().Next(5),
            FeelsLike = new Random().Next(5),
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        });
    }

    public override async Task GetCurrentWeatherStream(
        GerCurrentWeatherRequest request,
        IServerStreamWriter<WeatherResponse> responseStream,
        ServerCallContext context)
    {
        while(!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new WeatherResponse
            {
                Temperature = new Random().Next(5),
                FeelsLike = new Random().Next(5),
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            });
            await Task.Delay(1000);
        }
    }

    public override async Task<MultiWeatherResponse> GetMultiCurrentWeatherStream(
        IAsyncStreamReader<GerCurrentWeatherRequest> requestStream,
        ServerCallContext context)
    {
        var response = new MultiWeatherResponse { Weather = { } };

        await foreach (var request in requestStream.ReadAllAsync())
        {
            response.Weather.Add(new WeatherResponse
            {
                Temperature = new Random().Next(5),
                FeelsLike = new Random().Next(5),
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                City = request.City,
                Units = request.Units
            });
        }

        return response;
    }
}
