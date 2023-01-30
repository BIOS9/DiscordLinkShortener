namespace DiscordLinkShortener.Services.WebScraper.Scrapers.Aliexpress;

public readonly record struct AliexpressItemScrapeData(
    string Title,
    string ImageUrl,
    float Rating,
    int Orders,
    int Reviews,
    string ShopName,
    string ShopLink);