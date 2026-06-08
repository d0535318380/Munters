using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Models;

namespace Munters.Giphy.Handlers;

public record SearchQuery(string Text);

public record SearchQueryResult
{
    public ICollection<GiphyItemProjection> Items { get; init; } = new HashSet<GiphyItemProjection>();
}

public sealed class SearchQueryHandler : RequestHandlerBase<SearchQuery, SearchQueryResult>
{
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;
    private readonly GiphyApiClientOptions _options;
    private readonly IGiphyApiClient _client;

    public SearchQueryHandler(
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
            Expiration = _options.SearchExpiration
        };
    }

    protected override async Task<SearchQueryResult> ExecuteAsync(SearchQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var request = new SearchRequest(query.Text);
        
        var result = await _cache.GetOrCreateAsync(
                $"{nameof(SearchQueryHandler)}.{query.Text.Trim().ToUpperInvariant()}",
                async ctx => await _client.FetchSearchPagesAsync(request, _options.MaxParallelRequests, ctx).ConfigureAwait(false),
                _cacheOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }
}