using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using DiscordLinkShortener.Services.DiscordBot.Configuration;
using DiscordLinkShortener.Services.LinkShortener;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordLinkShortener.Services.DiscordBot;

public class DiscordBot : IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly DiscordBotOptions _botOptions;
    private readonly ILogger<DiscordBot> _logger;
    private readonly IEnumerable<ILinkShortener> _linkShorteners;

    private static readonly Regex _linkRegex = new(@"^https?:\/\/[^\s]+$", RegexOptions.IgnoreCase);
    
    private readonly IReadOnlyDictionary<LogSeverity, LogLevel>
        _logLevelMap = // Maps Discord.NET logging levels to Microsoft extensions logging levels.
            new Dictionary<LogSeverity, LogLevel>
            {
                { LogSeverity.Debug, LogLevel.Trace },
                { LogSeverity.Verbose, LogLevel.Debug },
                { LogSeverity.Info, LogLevel.Information },
                { LogSeverity.Warning, LogLevel.Warning },
                { LogSeverity.Error, LogLevel.Error },
                { LogSeverity.Critical, LogLevel.Critical },
            };

    public DiscordBot(
        DiscordSocketClient discordClient,
        IOptions<DiscordBotOptions> botOptions,
        ILogger<DiscordBot> logger,
        IEnumerable<ILinkShortener> linkShorteners)
    {
        _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        _botOptions = botOptions?.Value ?? throw new ArgumentNullException(nameof(botOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _linkShorteners = linkShorteners ?? throw new ArgumentNullException(nameof(linkShorteners));;

        _discordClient.Log += DiscordClientOnLog;
        _discordClient.MessageReceived += DiscordClientOnMessageReceived;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Discord bot starting...");
        await _discordClient.SetActivityAsync(new Game(_botOptions.StatusText));

        _logger.LogDebug("Logging in...");
        await _discordClient.LoginAsync(TokenType.Bot, _botOptions.Token);
        _logger.LogDebug("Connecting to websocket...");
        await _discordClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordClient.StopAsync();
    }

    private Task DiscordClientOnLog(LogMessage arg)
    {
        LogLevel level = _logLevelMap[arg.Severity];
        _logger.Log(level, arg.Exception, "{source} {message}", arg.Source, arg.Message);
        return Task.CompletedTask;
    }
    
    private async Task DiscordClientOnMessageReceived(SocketMessage arg)
    {
        if (arg.Author.Id == _discordClient.CurrentUser.Id)
            return;

        if (!_linkRegex.IsMatch(arg.Content)) // Ensure that message is just a link on its own
            return;

        var shortener = _linkShorteners.FirstOrDefault(x => x.CanShorten(arg.Content));
        if (shortener == null)
            return;

        string shortLink = await shortener.ShortenAsync(arg.Content);
        await arg.DeleteAsync();
        await arg.Channel.SendMessageAsync(shortLink);
    }
}