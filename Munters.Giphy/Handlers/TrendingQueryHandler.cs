using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Models;

namespace Munters.Giphy.Handlers;

public record TrendingQuery()
{
    public static TrendingQuery Default { get; } = new();
}

public sealed class TrendingQueryHandler : RequestHandlerBase<TrendingQuery, SearchQueryResult>
{
    private readonly IGiphyApiClient _client;
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public TrendingQueryHandler(
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

    protected override async Task<SearchQueryResult> ExecuteAsync(TrendingQuery _, CancellationToken cancellationToken)
    {
        var result = await _cache.GetOrCreateAsync(
                $"{nameof(TrendingQueryHandler)}",
                async ctx => await FetchAsync(ctx).ConfigureAwait(false),
                options: _cacheOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchAsync(CancellationToken ct)
    {
        var response = await _client.TrendingAsync(TrendingRequest.Default, ct).ConfigureAwait(false);

        if (response.NotHasPages)
        {
            return response.Adapt<SearchQueryResult>();
        }

        var result = await FetchPagesAsync(response, ct).ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchPagesAsync(SearchResponse sourceResponse, CancellationToken ct)
    {
        var page = sourceResponse.Pagination;
        var pageLimit = page.Count;

        var requests = Enumerable.Range(1, page.PagesCount - 1)
            .Select(i => new TrendingRequest(i * pageLimit, pageLimit));

        var responses = await Task.WhenAll(
            requests.Select(x => _client.TrendingAsync(x, ct).AsTask())
        ).ConfigureAwait(false);

        var items = responses
            .Union([sourceResponse])
            .SelectMany(x => x.Data.Adapt<GiphyItemProjection[]>())
            .ToArray();

        var result = new SearchQueryResult
        {
            Items = items
        };

        return result;
    }
}