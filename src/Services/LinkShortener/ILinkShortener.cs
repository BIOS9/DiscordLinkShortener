namespace DiscordLinkShortener.Services.LinkShortener;

public interface ILinkShortener
{
    bool CanShorten(string link);
    Task<string> Shorten(string link);
}