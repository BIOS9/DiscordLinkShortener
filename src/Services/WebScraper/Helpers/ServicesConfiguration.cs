using DiscordLinkShortener.Services.WebScraper.Scrapers;
using DiscordLinkShortener.Services.WebScraper.Scrapers.Aliexpress;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordLinkShortener.Services.WebScraper.Helpers;

public static class ServicesConfiguration
{
    public static void AddWebScrapers(this IServiceCollection services)
    {
        services.AddTransient<AliexpressScraper>();
        services.AddSingleton<ChromiumBrowserFactory>();
    }
}