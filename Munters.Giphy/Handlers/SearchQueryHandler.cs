using System.Linq;
using Mapster;
using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Models;

namespace Munters.Giphy.Handlers;

public record SearchQuery(string Text);

public record SearchQueryResult
{
    public ICollection<GiphyItemProjection> Items { get; init; } = new HashSet<GiphyItemProjection>();
};

public sealed class SearchQueryHandler : RequestHandlerBase<SearchQuery, SearchQueryResult>
{
    private readonly IGiphyApiClient _client;
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public SearchQueryHandler(
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
            Expiration = options.Value.SearchExpiration,
        };
    }

    protected override async Task<SearchQueryResult> ExecuteAsync(SearchQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var result = await _cache.GetOrCreateAsync(
                $"{nameof(SearchQueryHandler)}.{query.Text}",
                async ctx => await FetchAsync(query, ctx).ConfigureAwait(false),
                options: _cacheOptions, 
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchAsync(SearchQuery query, CancellationToken ct)
    {
        var request = new SearchRequest(query.Text);
        var sourceResponse = await _client.SearchAsync(request, ct).ConfigureAwait(false);

        if (sourceResponse.NotHasPages)
        {
            return sourceResponse.Adapt<SearchQueryResult>();
        }
        
        var result = await FetchPagesAsync(query, sourceResponse, ct).ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchPagesAsync(SearchQuery query, SearchResponse sourceResponse, CancellationToken ct)
    {
        var page = sourceResponse.Pagination;
        var pageLimit = page.Count;

        var requests = Enumerable.Range(1, page.PagesCount - 1)
            .Select(i => new SearchRequest(query.Text, i * pageLimit, pageLimit));
       
        var responses = await Task.WhenAll(
            requests.Select(x => _client.SearchAsync(x, ct).AsTask())
        ).ConfigureAwait(false);

        var items =  responses
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


