using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Elster
{
    public class Elster
    {
        private static readonly DiscordShardedClient discordClient =
            new(
                new DiscordSocketConfig
                {
                    LogLevel = Discord.LogSeverity.Info,
                    MessageCacheSize = 1000
                }
            );

        private static IHost ConfigureServices(string[] args, DiscordShardedClient client)
        {
            var builder = Host.CreateDefaultBuilder();

            builder.UseSerilog();
            builder.UseConsoleLifetime();

            builder.ConfigureServices(services =>
            {
                services
                    .AddSerilog()
                    .AddSingleton(client)
                    .AddSingleton<DiscordShardedClient>()
                    .AddHostedService<ElsterBot>();
            });

            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration
                    .AddJsonFile("elster.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets("01ce6809-c128-45c6-ac6d-256fa040eb40")
                    .AddEnvironmentVariables(prefix: "ELSTER_")
                    .AddCommandLine(args);
            });

            builder.ConfigureLogging(log =>
            {
                log.AddSimpleConsole().SetMinimumLevel(LogLevel.Information);
            });

            return builder.Build();
        }

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            IHost _host = ConfigureServices(args, discordClient);

            IConfiguration _config = _host.Services.GetRequiredService<IConfiguration>();

            if (_config["TOKEN"] is null) {
                throw new ApplicationException("Could not find required configuration key: TOKEN");
            }

            await _host.StartAsync();

            await Task.Delay(-1);
        }
    }
}
