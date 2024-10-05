using Serilog;

using GrpcPlayground.Services;
using GrpcPlayground.Interceptors;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

try
{
    Log.Information("Starting up application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddHttpClient();
    builder.Services.AddGrpc(options => options.Interceptors.Add<LoggerInterceptor>());

    var app = builder.Build();

    app.MapGrpcService<WeatherService>();
    app.MapGrpcService<ChatService>();
    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

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