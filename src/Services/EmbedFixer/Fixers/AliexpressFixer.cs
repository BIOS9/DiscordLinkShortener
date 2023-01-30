using System.Text.RegularExpressions;
using Discord;
using Discord.Rest;
using DiscordLinkShortener.Services.WebScraper.Scrapers.Aliexpress;

namespace DiscordLinkShortener.Services.EmbedFixer.Fixers;

public class AliexpressFixer : IEmbedFixer
{
    private static readonly Regex _itemLinkRegex = new(@"^https://www.aliexpress.com/item/[0-9]+.html$");
    private readonly AliexpressScraper _aliexpressScraper;

    public AliexpressFixer(AliexpressScraper aliexpressScraper)
    {
        _aliexpressScraper = aliexpressScraper ?? throw new ArgumentNullException(nameof(aliexpressScraper));
    }

    public bool CanFix(IUserMessage message)
    {
        return _itemLinkRegex.IsMatch(message.Content);
    }

    public async Task FixAsync(IUserMessage message)
    {
        if(!CanFix(message))
            throw new ArgumentException("Invalid Aliexpress link.");

        string link = message.Content;
        var scrapeData = await _aliexpressScraper.ScrapeItemAsync(link);
        message.ModifyAsync(msg =>
        {
            msg.Content = link + "\n" + scrapeData.Title;
        });
    }
}