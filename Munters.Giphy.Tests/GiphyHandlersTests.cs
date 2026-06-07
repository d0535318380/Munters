using LightResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Munters.Giphy.Client;
using Munters.Giphy.Handlers;
using Shouldly;

namespace Munters.Giphy.Tests;

public class GiphyHandlersTests(GiphyTestFixture fixture) : IClassFixture<GiphyTestFixture>
{
    [Fact]
    public async Task TrendingTestAsync()
    {
        var handler = fixture.Services.GetRequiredService<IRequestHandler<TrendingQuery, Result<SearchResult>>>();
        
        var result = await handler.Handle(TrendingQuery.Default, CancellationToken.None);
        var isSuccess = result.IsSuccess( out var value);
        
        isSuccess.ShouldBeTrue();
        value.ShouldNotBeNull();
        value.Items.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchTestAsync()
    {
        var handler = fixture.Services.GetRequiredService<IRequestHandler<SearchQuery, Result<SearchResult>>>();
        var result = await handler.Handle(new SearchQuery("GrindCoin"), cancellationToken: CancellationToken.None);
        
        var isSuccess = result.IsSuccess( out var value);
        
        isSuccess.ShouldBeTrue();
        value.ShouldNotBeNull();
        value.Items.ShouldNotBeEmpty();
    }
}