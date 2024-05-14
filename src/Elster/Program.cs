// using Discord.Commands;
// using Discord.WebSocket;
// using Elster.Common;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Hosting.Internal;
// using Serilog;

// namespace Elster
// {
//     public class Elster
//     {
//         /// <summary>
//         /// I mean... You know what this is about, right?
//         /// </summary>
//         public static async Task Main(string[] args)
//         {
//             Log.Logger = new LoggerConfiguration()
//                 .MinimumLevel.Debug()
//                 .Enrich.FromLogContext()
//                 .WriteTo.Console()
//                 .CreateLogger();

//             IHost host = ConfigureServices(args);

//             IConfiguration config = GetConfiguration(new HostingEnvironment());
//             if (config["TOKEN"] is null)
//             {
//                 throw new ApplicationException("Could not find required configuration key: TOKEN");
//             }

//             DiscordShardedClient client = host.Services.GetRequiredService<DiscordShardedClient>();

//             client.Log += LogAsync;

//             await host.RunAsync();
//         }

//         /// <summary>
//         /// Configures the services for the bot.
//         /// </summary>
//         /// <param name="args">The arguments passed to the Main method.</param>
//         /// <returns>The configured host.</returns>
//         private static IHost ConfigureServices(string[] args)
//         {
//             var builder = Host.CreateApplicationBuilder();

//             Logger.CreateLogger<Elster>();

//             builder
//                 .Services.AddSerilog()
//                 .AddSingleton(client)
//                 .AddSingleton<DiscordShardedClient>()
//                 .AddSingleton<CommandService>()
//                 .AddSingleton<CommandHandlerService>()
//                 .AddHostedService<ElsterBot>();

//             builder.Configuration.AddConfiguration(GetConfiguration(builder.Environment));

//             return builder.Build();
//         }

//         private static IConfigurationRoot GetConfiguration(IHostEnvironment environment)
//         {
//             ConfigurationBuilder configuration = new();

//             // configuration.SetBasePath();

//             configuration
//                 .AddEnvironmentVariables("ELSTER_")
//                 .AddJsonFile("elster.json", optional: true, reloadOnChange: false);

//             Log.Information($"Environment is {environment.EnvironmentName}");
//             if (environment.IsDevelopment())
//             {
//                 configuration.AddUserSecrets<Elster>();
//             }

//             return configuration.Build();
//         }
//     }
// }
