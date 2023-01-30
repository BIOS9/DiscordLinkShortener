using PuppeteerSharp;

namespace DiscordLinkShortener.Services.WebScraper;

public class ChromiumBrowserFactory
{
    private IBrowser? _cachedBrowser = null;
    
    public async Task<IBrowser> GetBrowserAsync()
    {
        // Lazy loading
        if (_cachedBrowser != null)
            return _cachedBrowser;
        
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        return _cachedBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
    }
}