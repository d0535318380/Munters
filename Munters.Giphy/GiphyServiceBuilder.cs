using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Munters.Giphy.Client;
using Munters.Giphy.Models;
using Refit;

namespace Munters.Giphy;

public static class GiphyServiceBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, // Giphy API often uses snake_case
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static readonly RefitSettings RefitSettings = new()
        { ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions) };

    public static IServiceCollection AddGiphyServices(this IServiceCollection services)
    {
        services
            .AddOptions<GiphyApiClientOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration
                    .GetSection(nameof(GiphyApiClientOptions))
                    .Bind(options);
            })
            .ValidateOnStart();

        services
            .AddTransient<GiphyAuthHandler>()
            .AddRefitClient<IGiphyApiClient>(RefitSettings)
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<GiphyApiClientOptions>>();
                client.BaseAddress = options.Value.BaseUrl;
            })
            .AddHttpMessageHandler<GiphyAuthHandler>()
            ;

        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<GiphyApiClientOptions>());

        services
            .AddLogging()
            .AddHybridCache();

        services
            .AddHttpClient()
            .ConfigureHttpClientDefaults(opts => opts.AddStandardResilienceHandler());

        return services;
    }
}