using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcPlayground.Services;

public class ChatService(ILogger<WeatherService> logger) : Chat.ChatBase
{
    private readonly ILogger<WeatherService> _logger = logger;

    public override async Task SendMessage(
        IAsyncStreamReader<ClientToServerMessage> requestStream,
        IServerStreamWriter<ServerToClientMessage> responseStream,
        ServerCallContext context)
    {
        var readTask = Read(requestStream, context);
        var sendTask = Send(responseStream, context);

        await Task.WhenAll(readTask, sendTask);
    }

    private async Task Read(
        IAsyncStreamReader<ClientToServerMessage> requestStream,
        ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested
               && await requestStream.MoveNext())
        {
            var request = requestStream.Current;
            _logger.LogInformation("Client said: {message}", request.Text);
        }
    }

    private async Task Send(
        IServerStreamWriter<ServerToClientMessage> responseStream,
        ServerCallContext context)
    {
        var pingCount = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new ServerToClientMessage
            {
                Text = $"Server said hi {++pingCount} times",
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            });
            await Task.Delay(1000);
        }
    }
}
