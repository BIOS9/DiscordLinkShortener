using System.Text.RegularExpressions;
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
        _logger.LogDebug("Scraping Aliexpress listing");
        
        if (!_itemLinkRegex.IsMatch(itemLink))
            throw new ArgumentException("Invalid Aliexpress item link.");

        var browser = await _browserFactory.GetBrowserAsync();
        var page = await browser.NewPageAsync();
        var response = await page.GoToAsync(itemLink);

        _logger.LogDebug("Aliexpress listing page loaded");
        
        if (!response.Ok)
            throw new Exception("Failed to load Aliexpress item page.");

        var titleElem = await page.QuerySelectorAsync(".product-title-text");
        var titleText = (await titleElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();

        var imageElem = await page.QuerySelectorAsync("meta[property='og:image']");
        var imageUrl = (await imageElem.GetPropertyAsync("content")).RemoteObject.Value.ToString();

        var ratingElem = await page.QuerySelectorAsync(".overview-rating-average");
        float rating = 0;
        if (ratingElem != null)
        {
            rating = float.Parse((await ratingElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString());
        }

        var ordersElem = await page.QuerySelectorAsync(".product-reviewer-sold");
        int orders = 0;
        if (ordersElem != null)
        {
            var ordersText = (await ordersElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
            orders = int.Parse(ordersText.Substring(0, ordersText.IndexOf(' ')));
        }

        var reviewsElem = await page.QuerySelectorAsync(".product-reviewer-reviews");
        int reviews = 0;
        if (reviewsElem != null)
        {
            var reviewsText = (await reviewsElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
            reviews = int.Parse(reviewsText.Substring(0, reviewsText.IndexOf(' ')));
        }

        var shopNameElem = await page.QuerySelectorAsync(".shop-name > a");
        var shopNameText = (await shopNameElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
        var shopLink = (await shopNameElem.GetPropertyAsync("href")).RemoteObject.Value.ToString();

        var items = await page.QuerySelectorAllAsync(".sku-property-list > li");

        
        var priceElem = await page.QuerySelectorAsync(".product-price-value");
        string? price = null;
        if (priceElem != null && items.Length == 0)
        {
            price = (await priceElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
        }

        var shippingElem = await page.QuerySelectorAsync(".dynamic-shipping-line > span > span > strong");
        string? shipping = null;
        if (items.Length == 0)
        {
            shipping = (await shippingElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();
        }

        _logger.LogDebug("Scrape complete");
        
        return new(titleText, imageUrl, rating, orders, reviews, shopNameText, shopLink, price, shipping);
    }
}