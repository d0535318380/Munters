namespace Munters.Giphy.Abstractions;

public interface IRequestHandler<in TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
} 