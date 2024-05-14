using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

using Elster.Helpers.Config;
using Elster.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Elster
{
    public class ElsterBot : IHostedService
    {
        public bool IsReady { get; private set; }
        private IUser? Owner { get; set; }

        private readonly IHost _host;

        private readonly DiscordShardedClient _client;
        private readonly CommandService _commandService;
        private readonly CommandHandlerService _commandHandler;
        private readonly RootSettings _settings;

        public ElsterBot(IHost host, RootSettings settings, DiscordShardedClient client, CommandService commandService, CommandHandlerService commandHandler)
        {
            _host = host;
            _settings = settings;

            _client = client;
            _client.Log += LogAsync;

            _commandService = commandService;
            _commandHandler = commandHandler;
        }

        public async Task StartAsync(CancellationToken token)
        {
            Log.Information("Connecting to Discord...");
            await _client.LoginAsync(TokenType.Bot, _settings.Bot.Token);
            await _client.StartAsync();

            await _commandHandler.InstallCommandsAsync();

            IsReady = true;
        }

        public async Task StopAsync(CancellationToken token)
        {
            Log.Information("Stopping Elster");
            await _client.LogoutAsync();
            await _client.StopAsync();
        }

        /// <summary>
        /// Installs all services required for the bot's lifetime.
        /// </summary>
        /// <returns>The services provider with all installed services.</returns>
        // private ServiceProvider InstallServices()
        // {
        //     var collection = new ServiceCollection()
        //         .AddSingleton(_client)
        //         .AddSingleton<DiscordShardedClient>()
        //         .AddSingleton(_commandService)
        //         .AddSingleton<CommandService>()
        //         .AddSingleton<CommandHandlerService>()
        //         .AddSingleton(this);

        //     ServiceProvider collection = collection.BuildServiceProvider();
        // }

        /// <summary>
        /// Attempts to obtain the owner linked to the bot through the application's information.
        /// </summary>
        /// <returns>The user who owns the bot.</returns>
        public async Task<IUser> GetOwner()
        {
            if (Owner is not null)
                return Owner;

            Log.Debug("Fetching owner information.");
            RestApplication app = await _client.GetApplicationInfoAsync();

            Owner = app.Owner; // It says it isn't nullable... But what if it's a team? Need confirmation
            return Owner;
        }

        /// <summary>
        /// Allow the Discord.NET framework to log to Serilog.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <returns>The completed task.</returns>
        private static async Task LogAsync(LogMessage message)
        {
            var severity = message.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            Log.Write(
                severity,
                message.Exception,
                "[{Source}] {Message}",
                message.Source,
                message.Message
            );
            await Task.CompletedTask;
        }
    }
}
