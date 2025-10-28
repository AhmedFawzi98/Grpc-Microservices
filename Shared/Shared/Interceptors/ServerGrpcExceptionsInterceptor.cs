using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Shared.Interceptors;

public class ServerGrpcExceptionsInterceptor(ILogger<ServerGrpcExceptionsInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException rpcException)
        {
            // Already an RpcException, just log and rethrow
            var message = $"Unary Call: {rpcException.Message}";
            logger.LogError(rpcException, "{Message}", message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Unknown, "unary call, an unexpected error occured in grpc service invoked"));
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
      IAsyncStreamReader<TRequest> requestStream,
      ServerCallContext context,
      ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(requestStream, context);
        }
        catch (RpcException rpcException)
        {
            // Already an RpcException, just log and rethrow
            var message = $"client streaming Call: {rpcException.Message}";
            logger.LogError(rpcException, "{Message}", message); throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Unknown, "client streaming call, an unexpected error occured in grpc service invoked"));
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (RpcException rpcException)
        {
            // Already an RpcException, just log and rethrow
            var message = $"server streamin Call: {rpcException.Message}";
            logger.LogError(rpcException, "{Message}", message); throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Unknown, "server streaming call, an unexpected error occured in grpc service invoked"));
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (RpcException rpcException)
        {
            // Already an RpcException, just log and rethrow
            var message = $"bidirectional streamin Call: {rpcException.Message}";
            logger.LogError(rpcException, "{Message}", message); throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new RpcException(new Status(StatusCode.Unknown, "bidirectional streaming call, an unexpected error occured in grpc service invoked"));
        }
    }

}
