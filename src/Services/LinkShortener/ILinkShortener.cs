namespace DiscordLinkShortener.Services.LinkShortener;

public interface ILinkShortener
{
    bool TryShorten(string link, out string shortLink);
}