using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Munters.Giphy.Tests;

public class GiphyTestFixture
{
    public GiphyTestFixture()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        Services = new ServiceCollection()
            .AddSingleton(Configuration)
            .AddGiphyServices()
            .BuildServiceProvider();
    }

    public IConfiguration Configuration { get; }
    public IServiceProvider Services { get; }
}