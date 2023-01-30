using System.Text.RegularExpressions;

namespace DiscordLinkShortener.Services.WebScraper.Scrapers.Aliexpress;

public class AliexpressScraper
{
    private static readonly Regex _itemLinkRegex = new(@"^https://www.aliexpress.com/item/[0-9]+.html$");
    
    private readonly ChromiumBrowserFactory _browserFactory;
    
    public AliexpressScraper(ChromiumBrowserFactory browserFactory)
    {
        _browserFactory = browserFactory ?? throw new ArgumentNullException(nameof(browserFactory));
    }

    public async Task<AliexpressItemScrapeData> ScrapeItemAsync(string itemLink)
    {
        if (!_itemLinkRegex.IsMatch(itemLink))
            throw new ArgumentException("Invalid Aliexpress item link.");

        var browser = await _browserFactory.GetBrowserAsync();
        var page = await browser.NewPageAsync();
        var response = await page.GoToAsync(itemLink);

        if (!response.Ok)
            throw new Exception("Failed to load Aliexpress item page.");

        var titleElem = await page.QuerySelectorAsync(".product-title-text");
        var titleText = (await titleElem.GetPropertyAsync("innerText")).RemoteObject.Value.ToString();

        return new(titleText);
    }
}