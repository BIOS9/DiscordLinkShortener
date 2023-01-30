using Microsoft.Extensions.Options;

namespace DiscordLinkShortener.Services.DiscordBot.Configuration;

public class DiscordBotOptionsValidation : IValidateOptions<DiscordBotOptions>
{
    public ValidateOptionsResult Validate(string name, DiscordBotOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Token))
        {
            return ValidateOptionsResult.Fail("Missing Discord bot token.");
        }

        return ValidateOptionsResult.Success;
    }
}