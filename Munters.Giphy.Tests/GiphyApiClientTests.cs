using Microsoft.Extensions.DependencyInjection;
using Munters.Giphy.Client;
using Shouldly;

namespace Munters.Giphy.Tests;

public class GiphyApiClientTests(GiphyTestFixture fixture) : IClassFixture<GiphyTestFixture>
{
    [Fact]
    public async Task TrendingTestAsync()
    {
        var client = fixture.Services.GetRequiredService<IGiphyApiClient>();
        
        var result = await client.TrendingAsync(TrendingRequest.Default, CancellationToken.None);
        
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task SearchTestAsync()
    {
        var client = fixture.Services.GetRequiredService<IGiphyApiClient>();
        var result = await client.SearchAsync(new SearchRequest("GrindCoin"), CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
    }
}