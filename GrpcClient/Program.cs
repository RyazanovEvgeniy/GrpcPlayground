using Serilog;

using GrpcClient.Services;
using GrpcClient.Interceptors;
using GrpcContracts;
using GrpcClient.Services.Interfaces;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

try
{
    Log.Information("Starting up application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddScoped<LoggerInterceptor>();
    builder.Services.AddScoped<IWeatherService, WeatherService>();
    builder.Services.AddGrpcClient<Weather.WeatherClient>((services, options) => options.Address = new Uri("https://localhost:3000"))
                    .AddInterceptor<LoggerInterceptor>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.MapGrpcService<WeatherService>();
    // app.MapGrpcService<ChatService>();

    app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}