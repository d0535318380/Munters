using Shouldly;

namespace Munters.Giphy.Tests;

[Collection(nameof(IntegrationTestsCollection))]
public class GiphyApiIntegrationTests(GiphyApiIntegrationFixture fixture)
{
    [Fact]
    public async Task TrendingSuccessTest()
    {
        // Act
        var client = fixture.Factory.CreateClient();
        var response = await client.GetAsync("/trending", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);
        content.ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("funny")]
    [InlineData("cat")]
    public async Task SearchSuccesTest(string searchTerm)
    {
        // Act
        var client = fixture.Factory.CreateClient();
        var response = await client.GetAsync($"/search/{searchTerm}", CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);
        content.ShouldNotBeNullOrEmpty();
    }
}