using Microsoft.Extensions.Options;
using Munters.Giphy.Models;

namespace Munters.Giphy.Client;

public class GiphyAuthHandler : DelegatingHandler
{
    private readonly GiphyApiClientOptions _options;

    public GiphyAuthHandler(IOptions<GiphyApiClientOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        
        _options = options.Value;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (request.RequestUri is not null)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);

            uriBuilder.Query = string.IsNullOrEmpty(uriBuilder.Query)
                ? "api_key=" + _options.ApiKey
                : string.Concat(uriBuilder.Query, "&api_key=", _options.ApiKey);

            request.RequestUri = uriBuilder.Uri;
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
