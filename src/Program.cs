﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordLinkShortener;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        // Create generic host and use startup service.
        // We use a startup service here instead of just adding each individual service
        // directly since some services require setup with things like configuration options.
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddHostedService<Startup>())
            .ConfigureAppConfiguration((context, builder) =>
                builder.AddUserSecrets<Program>());
    }
}