using LightResults;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Models;

namespace Munters.Giphy.Handlers;

public record TrendingQuery() : IRequest<Result<SearchResult>>
{
    public static TrendingQuery Default { get; } = new();
}

public sealed class TrendingHandler : QueryHandlerBase<TrendingQuery, SearchResult>
{
    private readonly IGiphyApiClient _client;
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public TrendingHandler(
        IOptions<GiphyApiClientOptions> options,
        IGiphyApiClient client,
        HybridCache cache,
        ILoggerFactory loggerFactory
    ) : base(loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(options);

        _client = client;
        _cache = cache;

        _cacheOptions = new HybridCacheEntryOptions()
        {
            Expiration = options.Value.TrendingExpiration,
        };
    }

    protected override async Task<SearchResult> ExecuteAsync(TrendingQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var request = new TrendingRequest();

        var response = await _cache.GetOrCreateAsync(
                $"{nameof(TrendingHandler)}",
                async ctx => await _client.TrendingAsync(request, ctx).ConfigureAwait(false),
                options: _cacheOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = new SearchResult(response.Data, response.Pagination);

        return result;
    }
}