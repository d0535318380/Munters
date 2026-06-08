using System.Collections.Concurrent;
using Munters.Giphy.Handlers;
using Munters.Giphy.Models;

namespace Munters.Giphy.Client;

internal static class GiphyApiClientExtensions
{
    public static async ValueTask<SearchQueryResult> FetchSearchPagesAsync(
        this IGiphyApiClient client, 
        SearchRequest request, 
        int maxParallelRequests = 4, 
        CancellationToken ct = default)
    {
        var sourceResponse = await client.SearchAsync(request, ct).ConfigureAwait(false);

        if (sourceResponse.IsSinglePage)
        {
            return sourceResponse.Adapt<SearchQueryResult>();
        }

        var result = await FetchPagesAsync(
            sourceResponse,
            (offset, limit) => new SearchRequest(request.Text, offset, limit),
            (req, token) => client.SearchAsync(req, token).AsTask(),
            maxParallelRequests,
            ct).ConfigureAwait(false);

        return result;
    }
    
    public static async ValueTask<SearchQueryResult> FetchTrendingPagesAsync(this IGiphyApiClient client, int maxParallelRequests = 4, CancellationToken ct = default)
    {
        var sourceResponse = await client.TrendingAsync(TrendingRequest.Default, ct).ConfigureAwait(false);

        if (sourceResponse.IsSinglePage)
        {
            return sourceResponse.Adapt<SearchQueryResult>();
        }

        var result = await FetchPagesAsync(
            sourceResponse,
            (offset, limit) => new TrendingRequest(offset, limit),
            (req, token) => client.TrendingAsync(req, token).AsTask(),
            maxParallelRequests,
            ct).ConfigureAwait(false);

        return result;
    }
    
    
    public static async ValueTask<SearchQueryResult> FetchPagesAsync<TRequest>(
        SearchResponse sourceResponse,
        Func<int, int, TRequest> requestFactory,
        Func<TRequest, CancellationToken, Task<SearchResponse>> fetchPage,
        int maxDegreeOfParallelism,
        CancellationToken ct)
    {
        var page = sourceResponse.Pagination;
        var pageLimit = page.Count;

        var requests = Enumerable.Range(1, page.PagesCount - 1)
            .Select(i => requestFactory(i * pageLimit, pageLimit))
            .ToArray();

        var responses = new ConcurrentBag<SearchResponse> { sourceResponse };

        await Parallel.ForEachAsync(requests,
            new ParallelOptions
            {
                CancellationToken = ct,
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            },
            async (request, token) =>
            {
                var response = await fetchPage(request, token).ConfigureAwait(false);
                responses.Add(response);
            }).ConfigureAwait(false);

        var items = responses
            .SelectMany(x => x.Data.Adapt<GiphyItemProjection[]>())
            .ToArray();

        return new SearchQueryResult { Items = items };
    }
}
