using Discord.Commands;
using Discord.WebSocket;
using Elster.Common.Folders;
using Elster.Helpers.Config;
using Elster.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Elster;

/// <summary>
/// I just made this class for my sanity.
/// This should go into the Program.cs tho.
/// </summary>
class Elster
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

        IConfigurationRoot config = GetConfiguration(hostBuilder.Environment);
        RootSettings settings =
            config.Get<RootSettings>()
            ?? throw new ApplicationException(
                "Invalid configuration. This more likely mean a configuration key is missing."
            );
        hostBuilder.Configuration.AddConfiguration(config);

        DiscordShardedClient client = new(
            new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Debug,
                MessageCacheSize = 1000,
                GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
            }
        );

        hostBuilder
            .Services.AddSingleton(client)
            .AddSingleton<DiscordShardedClient>()
            .AddSingleton(settings)
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlerService>()
            .AddHostedService<ElsterBot>();

        var host = hostBuilder.Build();

        try
        {
            await host.RunAsync();
        }
        catch (Exception)
        {
            Log.Fatal("Host has been terminated unexpectedly!");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IConfigurationRoot GetConfiguration(IHostEnvironment environment)
    {
        ConfigurationBuilder configuration = new();

        configuration
            .SetBasePath(FoldersUtils.GetElsterCoreConfigPath(true))
            .AddEnvironmentVariables("Elster_")
            .AddJsonFile("elster.json", optional: true, reloadOnChange: false);

        Log.Debug($"Environment is {environment.EnvironmentName}");
        if (environment.IsDevelopment())
        {
            configuration.AddUserSecrets<Elster>();
        }

        return configuration.Build();
    }
}
