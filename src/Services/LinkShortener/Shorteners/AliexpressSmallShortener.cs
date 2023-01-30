using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DiscordLinkShortener.Services.LinkShortener.Shorteners;

public class AliexpressSmallShortener : ILinkShortener
{
    private static readonly Regex _aliLinkRegex =
        new(@"^https?:\/\/a.aliexpress.com\/[_a-z0-9]+$", RegexOptions.IgnoreCase);

    private readonly ILogger<AliexpressSmallShortener> _logger;

    public AliexpressSmallShortener(ILogger<AliexpressSmallShortener> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool CanShorten(string link) => _aliLinkRegex.IsMatch(link);

    public async Task<string> ShortenAsync(string link)
    {
        _logger.LogInformation("Shortening small Aliexpress link");
        
        var m = _aliLinkRegex.Match(link);
        if (!m.Success)
            throw new ArgumentException("Invalid small Aliexpress link");

        var response = await new HttpClient().GetAsync(link);
        if (response.StatusCode != HttpStatusCode.Found)
        {
            _logger.LogWarning("Was expecting 302 redirect for small Aliexpress link, got {Code}", response.StatusCode);
            throw new WebException("Small Aliexpress link not redirecting properly");
        }

        if (response?.Headers?.Location == null || string.IsNullOrEmpty(response.Headers.Location.ToString()))
        {
            _logger.LogWarning("Missing location header for small Aliexpress link");
            throw new ArgumentException("Missing location header for small Aliexpress link");
        }

        return response.Headers.Location.ToString();
    }
}