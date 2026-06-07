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
        
        var result = await client.TrendingAsync(TrendingRequest.Default);
        
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task SearchTestAsync()
    {
        var client = fixture.Services.GetRequiredService<IGiphyApiClient>();
        var result = await client.SearchAsync(new SearchRequest("GrindCoin"));
        
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
    }
}