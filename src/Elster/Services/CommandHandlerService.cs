using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;

namespace Elster.Services;

public class CommandHandlerService(
    IServiceProvider serviceProvider,
    DiscordShardedClient client,
    CommandService commands
)
{
    private readonly DiscordShardedClient _client = client;
    private readonly CommandService _commands = commands;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;

        // Here we discover all of the command modules in the entry
        // assembly and load them. Starting from Discord.NET 2.0, a
        // service provider is required to be passed into the
        // module registration method to inject the
        // required dependencies.
        //
        // If you do not use Dependency Injection, pass null.
        // See Dependency Injection guide for more information.
        await _commands.AddModulesAsync(
            assembly: Assembly.GetEntryAssembly(),
            services: _serviceProvider
        );

        Log.Debug("Installed command handler.");
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        if (messageParam is not SocketUserMessage message)
            return;

        // Don't process the command if the message is nothing (Missing intent)
        if (message.Content.Length == 0)
            return;

        Log.Debug("Started handling command.");

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new ShardedCommandContext(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commands.ExecuteAsync(context: context, argPos: argPos, services: _serviceProvider);
    }
}
