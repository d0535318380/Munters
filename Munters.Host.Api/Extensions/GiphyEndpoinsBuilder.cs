using LightResults;
using Munters.Giphy.Abstractions;
using Munters.Giphy.Handlers;

namespace Munters.Host.Api.Extensions;

internal static class GiphyEndpoinsBuilder
{
    public static IEndpointRouteBuilder MapGiphyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/trending", async (IRequestHandler<TrendingQuery, Result<SearchQueryResult>> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(TrendingQuery.Default, cancellationToken).ConfigureAwait(false);
    
            return result.IsSuccess(out var value) ? Results.Ok(value.Items) : Results.BadRequest();
        });

        endpoints.MapGet("/search/{text}", async (string text, IRequestHandler<SearchQuery, Result<SearchQueryResult>> handler, CancellationToken cancellationToken) =>
        {
            var query = new SearchQuery(text);
            var result = await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
    
            return result.IsSuccess(out var value) ? Results.Ok(value.Items) : Results.BadRequest();
        });
        
        return endpoints;
    }
}