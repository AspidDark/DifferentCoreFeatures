using Grpc.Core;
using Grpc.Core.Interceptors;


public class GrpcRequestLoggingInterceptor(ILogger<GrpcRequestLoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Incoming gRPC Request: {Method} | Request: {@Request}", context.Method, request);
        try
        {
            var response = await continuation(request, context);
            logger.LogInformation("gRPC Response: {Method} | Response: {@Response}", context.Method, response);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "gRPC Error in {Method}", context.Method);
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Server Streaming Request: {Method} | Payload: {@Request}", context.Method, request);

        await continuation(request, responseStream, context);

        logger.LogInformation("Server Streaming Response complete: {Method}", context.Method);
    }
}


builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcRequestLoggingInterceptor>();
});