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

    public async Task FixAsync(IUserMessage message, IMessage originalMessage)
    {
        if(!CanFix(message))
            throw new ArgumentException("Invalid Aliexpress link.");

        string link = message.Content;
        var scrapeData = await _aliexpressScraper.ScrapeItemAsync(link);
        
        string ratingString = string.Concat(Enumerable.Repeat(":star:", (int)Math.Ceiling(scrapeData.Rating))) + "  " +
                              scrapeData.Rating;
        
        var embed = new EmbedBuilder()
            .WithTitle(scrapeData.Title)
            .WithDescription($"Rating: **{ratingString}**\n" +
                             $"Orders: **{scrapeData.Orders}**\n" +
                             $"Reviews: **{scrapeData.Reviews}**\n")
            .WithColor(new Color(217, 44, 4)) // Ali color :)
            .WithUrl(link)
            .WithImageUrl(scrapeData.ImageUrl)
            .WithCurrentTimestamp()
            .WithAuthor(x =>
            {
                x.Name = $"{originalMessage.Author.Username}#{originalMessage.Author.DiscriminatorValue}";
                x.IconUrl = originalMessage.Author.GetAvatarUrl(ImageFormat.Auto, 512);
            })
            .WithFooter("Aliexpress");

        await message.ModifyAsync(msg =>
        {
            msg.Embed = embed.Build();
            msg.Content = string.Empty;
        });
    }
}