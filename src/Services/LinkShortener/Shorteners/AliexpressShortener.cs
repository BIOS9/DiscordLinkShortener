using System.Text.RegularExpressions;

namespace DiscordLinkShortener.Services.LinkShortener.Shorteners;

public class AliexpressShortener : ILinkShortener
{
    private static readonly Regex _aliLinkRegex =
        new(@"^https?:\/\/(?:[^\s]+.)?aliexpress.com\/item\/([0-9]+).+$", RegexOptions.IgnoreCase);

    public bool CanShorten(string link) => _aliLinkRegex.IsMatch(link);

    public Task<string> ShortenAsync(string link)
    {
        var m = _aliLinkRegex.Match(link);
        if (!m.Success)
            throw new ArgumentException("Invalid Aliexpress link.");

        return Task.FromResult($"https://aliexpress.com/item/{m.Groups[1].Value}.html");
    }
}