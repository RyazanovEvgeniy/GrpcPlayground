using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcClient.Interceptors;

public class LoggerInterceptor(ILogger<LoggerInterceptor> logger) : Interceptor
{
    private readonly ILogger<LoggerInterceptor> _logger = logger;

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Starting call. Type: {type}. Request: {request}. Response: {response}",
                                context.Method.Type, typeof(TRequest), typeof(TResponse));

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(HandleResponse(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
    }

    private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> tResponse)
    {
        try
        {
            var response = await tResponse;
            _logger.LogInformation("Response received: {response}", response);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error: {message}", ex.Message);
            return default;
        }
    }
}