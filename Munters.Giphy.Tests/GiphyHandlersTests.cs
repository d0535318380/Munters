using LightResults;
using Microsoft.Extensions.DependencyInjection;
using Munters.Giphy.Abstractions;
using Munters.Giphy.Handlers;
using Shouldly;

namespace Munters.Giphy.Tests;

public class GiphyHandlersTests(GiphyTestFixture fixture) : IClassFixture<GiphyTestFixture>
{
    [Fact]
    public async Task TrendingSuccesTest()
    {
        var handler = fixture.Services.GetRequiredService<IRequestHandler<TrendingQuery, Result<SearchQueryResult>>>();

        var result = await handler.HandleAsync(TrendingQuery.Default, CancellationToken.None);
        var isSuccess = result.IsSuccess(out var value);

        isSuccess.ShouldBeTrue();
        value.ShouldNotBeNull();
        value.Items.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SearchSuccessTest()
    {
        var handler = fixture.Services.GetRequiredService<IRequestHandler<SearchQuery, Result<SearchQueryResult>>>();
        var result = await handler.HandleAsync(new SearchQuery("GrindCoin"), CancellationToken.None);

        var isSuccess = result.IsSuccess(out var value);

        isSuccess.ShouldBeTrue();
        value.ShouldNotBeNull();
        value.Items.ShouldNotBeEmpty();
    }
}