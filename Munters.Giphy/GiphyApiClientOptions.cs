using System.ComponentModel.DataAnnotations;

namespace Munters.Giphy;

public sealed class GiphyApiClientOptions
{
    [Required] [MinLength(10)] public string ApiKey { get; set; } = string.Empty;

    [Required] public Uri BaseUrl { get; set; } = new("https://api.giphy.com");

    [Required] public int SearchExpirationsInHours { get; set; } = 24;

    [Required] public int TrendingExpirationsInMinutes { get; set; } = 15;
    
    public int MaxParallelRequests { get; set; } = 4;
    
    public TimeSpan SearchExpiration => TimeSpan.FromHours(SearchExpirationsInHours);
    public TimeSpan TrendingExpiration => TimeSpan.FromMinutes(TrendingExpirationsInMinutes);
}