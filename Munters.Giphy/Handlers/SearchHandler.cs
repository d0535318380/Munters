using LightResults;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Models;

namespace Munters.Giphy.Handlers;

public record SearchQuery(string Text) : IRequest<Result<SearchResult>>;
public record SearchResult(GiphyItem[] Items, PaginationItem Pagination);

public sealed class SearchHandler : QueryHandlerBase<SearchQuery, SearchResult>
{
    private readonly IGiphyApiClient _client;
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public SearchHandler(
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

    protected override async Task<SearchResult> ExecuteAsync(SearchQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var request = new SearchRequest(query.Text);
        
        var response = await _cache.GetOrCreateAsync(
                $"{nameof(SearchHandler)}.{query.Text}",
                async ctx => await _client.SearchAsync(request, ctx).ConfigureAwait(false),
                options: _cacheOptions, 
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = new SearchResult(response.Data, response.Pagination);
        
        return result;
    }
}


