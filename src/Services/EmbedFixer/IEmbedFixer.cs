using Discord;

namespace DiscordLinkShortener.Services.EmbedFixer;

public interface IEmbedFixer
{
    bool CanFix(IUserMessage message);
    Task FixAsync(IUserMessage message);
}