using Munters.Giphy.Abstractions;
using Munters.Giphy.Client;
using Munters.Giphy.Handlers;
using Refit;

namespace Munters.Giphy;

public static class GiphyServiceBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, // Giphy API often uses snake_case
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly RefitSettings RefitSettings = new()
        { ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions) };

    public static IServiceCollection AddGiphyServices(this IServiceCollection services)
    {
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

        services.Scan(scan => scan
            .FromAssemblyOf<GiphyApiClientOptions>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

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
            .AddLogging()
            .AddHybridCache();

        services
            .AddHttpClient()
            .ConfigureHttpClientDefaults(opts => opts.AddStandardResilienceHandler());

        services
            .AddMapster();

        TypeAdapterConfig.GlobalSettings.NewConfig<SearchResponse, SearchQueryResult>()
            .Map(dest => dest.Items, src => src.Data);

        return services;
    }
}