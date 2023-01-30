using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace DiscordLinkShortener.Services.WebScraper;

public class CachedBrowserFactory : IBrowserFactory
{
    private readonly ILogger<CachedBrowserFactory> _logger;
    
    private IBrowser? _cachedBrowser = null;

    public CachedBrowserFactory(ILogger<CachedBrowserFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IBrowser> GetBrowserAsync()
    {
        _logger.LogDebug("Loading embedded browser");
        // Lazy loading
        if (_cachedBrowser != null)
        {
            _logger.LogDebug("Browser cache hit");
            return _cachedBrowser;
        }
        
        _logger.LogDebug("Browser case miss, launching browser");
        
        return _cachedBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new [] { "--no-sandbox" }
        });
    }
}