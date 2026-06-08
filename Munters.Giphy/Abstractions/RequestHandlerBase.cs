using LightResults;

namespace Munters.Giphy.Abstractions;

public abstract partial class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
{
    private readonly ILogger _logger;

    protected RequestHandlerBase(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        LogStartHandling(typeof(TRequest).Name);

        try
        {
            var result = await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            LogErrorHandling(typeof(TRequest).Name, ex);

            return Result.Failure<TResponse>(ex);
        }
        finally
        {
            LogEndHandlingQ(typeof(TRequest).Name);
        }
    }

    protected abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);

    [LoggerMessage(LogLevel.Information, "Start handling query {QueryType}")]
    partial void LogStartHandling(string queryType);

    [LoggerMessage(LogLevel.Error, "Error handling query {QueryType}")]
    partial void LogErrorHandling(string queryType, Exception exception);

    [LoggerMessage(LogLevel.Information, "End handling query {QueryType}")]
    partial void LogEndHandlingQ(string queryType);
}