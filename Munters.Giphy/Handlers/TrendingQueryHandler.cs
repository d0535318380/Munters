using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;

namespace Munters.Giphy.Handlers;

public record TrendingQuery
{
    public static TrendingQuery Default { get; } = new();
}

public sealed class TrendingQueryHandler : RequestHandlerBase<TrendingQuery, SearchQueryResult>
{
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;
    private readonly GiphyApiClientOptions _options;
    private readonly IGiphyApiClient _client;

    public TrendingQueryHandler(
        IOptions<GiphyApiClientOptions> options,
        IGiphyApiClient client,
        HybridCache cache,
        ILoggerFactory loggerFactory
    ) : base(loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
        _client = client;
        _cache = cache;

        _cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = options.Value.TrendingExpiration
        };
    }

    protected override async Task<SearchQueryResult> ExecuteAsync(TrendingQuery _, CancellationToken cancellationToken)
    {
        var result = await _cache.GetOrCreateAsync(
                $"{nameof(TrendingQueryHandler)}",
                async ctx => await _client.FetchTrendingPagesAsync(_options.MaxParallelRequests, ctx).ConfigureAwait(false),
                _cacheOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }
}