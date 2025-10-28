using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Shared.Interceptors;

public class ClientLoggingInterceptor : Interceptor
{
    private readonly ILogger<ClientLoggingInterceptor> _logger;

    public ClientLoggingInterceptor(ILogger<ClientLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();

        LogRequest(context.Method.FullName, request);

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponseAsync(call.ResponseAsync, context.Method.FullName, stopwatch),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose
        );
    }

    private async Task<TResponse> HandleResponseAsync<TResponse>(
        Task<TResponse> responseTask,
        string method,
        Stopwatch stopwatch)
    {
        try
        {
            var response = await responseTask;
            stopwatch.Stop();
            LogResponse(method, stopwatch.ElapsedMilliseconds, true, response);
            return response;
        }
        catch (RpcException rpcEx)
        {
            stopwatch.Stop();
            LogResponse<object>(method, stopwatch.ElapsedMilliseconds, false, null);
            throw; 
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogResponse<object>(method, stopwatch.ElapsedMilliseconds, false, null);
            throw; 
        }
    }

    private void LogRequest<TRequest>(string method, TRequest request)
    {
        _logger.LogInformation(
            "Client gRPC Request: {Method}\nRequest:\n{Request}",
            method,
            request?.ToString());
    }

    private void LogResponse<TResponse>(string method, long durationMs, bool success, TResponse response)
    {
        _logger.LogInformation(
            "Client gRPC Response: {Method}\nDuration: {Duration}ms\nSuccess: {Success}\nResponse:\n{Response}",
            method,
            durationMs,
            success,
            response?.ToString());
    }
}
