using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DiscordLinkShortener.Services.LinkShortener.Shorteners;

public class AliexpressShortener : ILinkShortener
{
    private static readonly Regex _aliLinkRegex =
        new(@"^https?:\/\/(?:[^\s]+.)?aliexpress.com\/item\/([0-9]+).+$", RegexOptions.IgnoreCase);

    private readonly ILogger<AliexpressShortener> _logger;

    public AliexpressShortener(ILogger<AliexpressShortener> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool CanShorten(string link) => _aliLinkRegex.IsMatch(link);

    public Task<string> ShortenAsync(string link)
    {
        _logger.LogInformation("Shortening Aliexpress link");
        
        var m = _aliLinkRegex.Match(link);
        if (!m.Success)
            throw new ArgumentException("Invalid Aliexpress link.");

        return Task.FromResult($"https://www.aliexpress.com/item/{m.Groups[1].Value}.html");
    }
}