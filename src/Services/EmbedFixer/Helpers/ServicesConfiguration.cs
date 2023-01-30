using DiscordLinkShortener.Services.EmbedFixer.Fixers;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordLinkShortener.Services.EmbedFixer.Helpers;

public static class ServicesConfiguration
{
    public static void AddEmbedFixers(this IServiceCollection services)
    {
        services.AddTransient<IEmbedFixer, AliexpressFixer>();
    }
}