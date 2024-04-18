using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Elster
{
    public class ElsterBot(DiscordShardedClient client, IConfiguration config) : IHostedService
    {
        private readonly DiscordShardedClient _client = client;
        private readonly IConfiguration _config = config;

        public async Task StartAsync(CancellationToken token)
        {
            Log.Information("Connecting to Discord...");
            await _client.LoginAsync(Discord.TokenType.Bot, _config.GetValue<string>("TOKEN"));
            await _client.StartAsync();
            Log.Debug("Bot has connected to Discord. Blocking loop.");
        }

        public async Task StopAsync(CancellationToken token)
        {
            Log.Information("Stopping Elster");
            await _client.LogoutAsync();
            await _client.StopAsync();
            await Log.CloseAndFlushAsync();
        }
    }
}
