﻿using Discord;
using Discord.WebSocket;
using DiscordLinkShortener.Services.DiscordBot.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordLinkShortener.Services.DiscordBot;

public class DiscordBot : IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly DiscordBotOptions _botOptions;
    private readonly ILogger<DiscordBot> _logger;

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
        ILogger<DiscordBot> logger)
    {
        _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        _botOptions = botOptions?.Value ?? throw new ArgumentNullException(nameof(botOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _discordClient.Log += DiscordClientOnLog;
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
        _logger.Log(level, "{source} {message}", arg.Source, arg.Message, arg.Exception);
        return Task.CompletedTask;
    }
}