using Refit;

namespace Munters.Giphy.Client;

public interface IGiphyApiClient
{
    [Get("/v1/gifs/search")]
    ValueTask<SearchResponse> SearchAsync(SearchRequest query, CancellationToken ct = default);

    [Get("/v1/gifs/trending")]
    ValueTask<SearchResponse> TrendingAsync(TrendingRequest query, CancellationToken ct = default);
}