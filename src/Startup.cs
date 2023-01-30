using DiscordLinkShortener.Services.DiscordBot.Helpers;
using DiscordLinkShortener.Services.LinkShortener.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace DiscordLinkShortener;

public class Startup : IHostedService
{
    private readonly IConfiguration _configuration;
    private IEnumerable<IHostedService> _runningServices;
        
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Application started.");

        // Set up services for dependency injection.
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();
            
        _runningServices = provider.GetServices<IHostedService>();
        return Task.WhenAll(_runningServices.Select(s => 
            s.StartAsync(cancellationToken)));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Application stopping.");
        return Task.WhenAll(_runningServices.Select(s => 
            s.StopAsync(cancellationToken)));
    }
        
    /// <summary>
    /// Configures services to be dependency injected.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(x => x.AddSerilog());
        services.AddDiscordBot(_configuration);
        services.AddLinkShorteners();
    }
}