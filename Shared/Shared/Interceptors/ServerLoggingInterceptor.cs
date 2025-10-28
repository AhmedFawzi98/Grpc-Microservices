using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Shared.Interceptors;

public class ServerLoggingInterceptor : Interceptor
{
    private readonly ILogger<ServerLoggingInterceptor> _logger;

    public ServerLoggingInterceptor(ILogger<ServerLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        LogRequest(context.Method, request, context);

        try
        {
            var response = await continuation(request, context);
            stopwatch.Stop();
            LogResponse(context.Method, stopwatch.ElapsedMilliseconds, true, response);
            return response;
        }
        catch (RpcException rpcEx)
        {
            stopwatch.Stop();
            LogResponse<object>(context.Method, stopwatch.ElapsedMilliseconds, false, null);
            throw; 
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogResponse<object>(context.Method, stopwatch.ElapsedMilliseconds, false, null);
            throw;
        }
    }

    private void LogRequest<TRequest>(string method, TRequest request, ServerCallContext context)
    {
        _logger.LogInformation(
            "gRPC Request: {Method}\nPeer: {Peer}\nRequest:\n{Request}",
            method,
            context.Peer,
            request?.ToString());
    }

    private void LogResponse<TResponse>(string method, long durationMs, bool success, TResponse response)
    {
        _logger.LogInformation(
            "gRPC Response: {Method}\nDuration: {Duration}ms\nSuccess: {Success}\nResponse:\n{Response}",
            method,
            durationMs,
            success,
            response?.ToString());
    }
}
