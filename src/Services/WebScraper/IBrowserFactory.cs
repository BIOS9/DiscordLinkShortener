using PuppeteerSharp;

namespace DiscordLinkShortener.Services.WebScraper;

public interface IBrowserFactory
{
    Task<IBrowser> GetBrowserAsync();
}