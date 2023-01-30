﻿using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DiscordLinkShortener.Services.WebScraper.Scrapers.Aliexpress;

public class AliexpressScraper
{
    private static readonly Regex _itemLinkRegex = new(@"^https://www.aliexpress.com/item/[0-9]+.html$");
    
    private readonly ILogger<AliexpressScraper> _logger;
    private readonly IBrowserFactory _browserFactory;
    
    public AliexpressScraper(ILogger<AliexpressScraper> logger, IBrowserFactory browserFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _browserFactory = browserFactory ?? throw new ArgumentNullException(nameof(browserFactory));
    }

    public async Task<AliexpressItemScrapeData> ScrapeItemAsync(string itemLink)
    {
        _logger.LogInformation("Scraping Aliexpress listing");
        
        if (!_itemLinkRegex.IsMatch(itemLink))
            throw new ArgumentException("Invalid Aliexpress item link.");

        var browser = await _browserFactory.GetBrowserAsync();
        var page = await browser.NewPageAsync();
        var response = await page.GoToAsync(itemLink);

        if (!response.Ok)
            throw new Exception("Failed to load Aliexpress item page.");

        var titleElem = await page.QuerySelectorAsync(".product-title-text");
        var titleText = (await titleElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();

        var imageElem = await page.QuerySelectorAsync("meta[property='og:image']");
        var imageUrl = (await imageElem.GetPropertyAsync("content")).RemoteObject.Value.ToString();

        var ratingElem = await page.QuerySelectorAsync(".overview-rating-average");
        var rating = float.Parse((await ratingElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString());
        
        var ordersElem = await page.QuerySelectorAsync(".product-reviewer-sold");
        var ordersText = (await ordersElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
        var orders = int.Parse(ordersText.Substring(0, ordersText.IndexOf(' ')));
        
        var ratingsElem = await page.QuerySelectorAsync(".product-reviewer-reviews");
        var ratingsText = (await ratingsElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
        var ratings = int.Parse(ratingsText.Substring(0, ordersText.IndexOf(' ')));

        return new(titleText, imageUrl, rating, orders, ratings);
    }
}