using MediatR;
using Munters.Giphy.Handlers;

namespace Munters.Host.Api.Extensions;

public static class GiphyEndpoinsBuilder
{
    public static IEndpointRouteBuilder MapGiphyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/trending", async (IMediator mediator) =>
        {
            var result = await mediator.Send(TrendingQuery.Default).ConfigureAwait(false);
    
            return result.IsSuccess(out var value) ? Results.Ok(value) : Results.BadRequest();
        });

        endpoints.MapGet("/search/{text}", async (string text, IMediator mediator) =>
        {
            var query = new SearchQuery(text);
            var result = await mediator.Send(query).ConfigureAwait(false);
    
            return result.IsSuccess(out var value) ? Results.Ok(value) : Results.BadRequest();
        });
        
        return endpoints;
    }
}