using DiscordLinkShortener.Services.LinkShortener.Shorteners;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordLinkShortener.Services.LinkShortener.Helpers;

public static class ServicesConfiguration
{
    public static void AddLinkShorteners(this IServiceCollection services)
    {
        services.AddTransient<AliexpressShortener>();
    }
}