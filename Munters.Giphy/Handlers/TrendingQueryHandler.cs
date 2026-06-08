﻿using System.Collections.Concurrent;
 using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Models;

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
                async ctx => await FetchAsync(ctx).ConfigureAwait(false),
                _cacheOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchAsync(CancellationToken ct)
    {
        var response = await _client.TrendingAsync(TrendingRequest.Default, ct).ConfigureAwait(false);

        if (response.NotHasPages) return response.Adapt<SearchQueryResult>();

        var result = await FetchPagesAsync(response, ct).ConfigureAwait(false);

        return result;
    }

    private async ValueTask<SearchQueryResult> FetchPagesAsync(SearchResponse sourceResponse, CancellationToken ct)
    {
        var page = sourceResponse.Pagination;
        var pageLimit = page.Count;

        var requests = Enumerable.Range(1, page.PagesCount - 1)
            .Select(i => new TrendingRequest(i * pageLimit, pageLimit))
            .ToArray();

        var responses = new ConcurrentBag<SearchResponse>() { sourceResponse };

        await Parallel.ForEachAsync(requests,
            new ParallelOptions
            {
                CancellationToken = ct,
                MaxDegreeOfParallelism = _options.MaxParallelRequests
            },
            async (request, token) =>
            {
                var response = await _client.TrendingAsync(request, token).ConfigureAwait(false);
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