using Grpc.Core;
using Grpc.Core.Interceptors;

using GrpcServer.Helpers;

namespace GrpcServer.Interceptors;

public class LoggerInterceptor(ILogger<LoggerInterceptor> logger) : Interceptor
{
    private readonly ILogger<LoggerInterceptor> _logger = logger;
    private readonly Guid _correlationId = Guid.NewGuid();

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Procedure call. Type: {method}. Request: {request}. Response: {response}",
                                MethodType.Unary,
                                typeof(TRequest),
                                typeof(TResponse));
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error: {message}", e.Message);
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
    }

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Procedure call. Type: {method}. Request: {request}. Response: {response}",
                                MethodType.ClientStreaming,
                                typeof(TRequest),
                                typeof(TResponse));
        return base.ClientStreamingServerHandler(requestStream, context, continuation);
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Procedure call. Type: {method}. Request: {request}. Response: {response}",
                                MethodType.ServerStreaming,
                                typeof(TRequest),
                                typeof(TResponse));
        return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Procedure call. Type: {method}. Request: {request}. Response: {response}",
                                MethodType.DuplexStreaming,
                                typeof(TRequest),
                                typeof(TResponse));
        return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
    }
}