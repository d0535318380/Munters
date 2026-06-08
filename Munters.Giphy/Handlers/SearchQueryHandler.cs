﻿using System.Collections.Concurrent;
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

        var normalizedText = NormalizeSearchText(query.Text);

        var result = await _cache.GetOrCreateAsync(
                $"{nameof(SearchQueryHandler)}.{normalizedText}",
                async ctx => await FetchAsync(query, ctx).ConfigureAwait(false),
                _cacheOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private static string NormalizeSearchText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        return string.Join(' ', text
            .Trim()
            .ToUpperInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private async ValueTask<SearchQueryResult> FetchAsync(SearchQuery query, CancellationToken ct)
    {
        var request = new SearchRequest(query.Text);
        var sourceResponse = await _client.SearchAsync(request, ct).ConfigureAwait(false);

        if (sourceResponse.NotHasPages) return sourceResponse.Adapt<SearchQueryResult>();

        var result = await FetchPagesAsync(query, sourceResponse, ct).ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchPagesAsync(SearchQuery query, SearchResponse sourceResponse,
        CancellationToken ct)
    {
        var page = sourceResponse.Pagination;
        var pageLimit = page.Count;

        var requests = Enumerable.Range(1, page.PagesCount - 1)
            .Select(i => new SearchRequest(query.Text, i * pageLimit, pageLimit))
            .ToArray();

        var responses = new ConcurrentBag<SearchResponse> { sourceResponse };

        await Parallel.ForEachAsync(requests,
            new ParallelOptions
            {
                CancellationToken = ct,
                MaxDegreeOfParallelism = _options.MaxParallelRequests
            },
            async (request, token) =>
            {
                var response = await _client.SearchAsync(request, token).ConfigureAwait(false);
                
                responses.Add(response);
            }).ConfigureAwait(false);

        var items = responses
            .SelectMany(x => x.Data.Adapt<GiphyItemProjection[]>())
            .ToArray();

        var result = new SearchQueryResult
        {
            Items = items
        };

        return result;
    }
}