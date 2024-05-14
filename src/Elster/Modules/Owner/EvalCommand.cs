using Discord.Commands;
using Elster.Common.Extensions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Elster.Modules.Owner;

public abstract class OwnerModule
{
    [Group]
    public class EvalCommand(IServiceProvider services) : ModuleBase<ShardedCommandContext>
    {
        private readonly IServiceProvider _services = services;

        [Command("eval")]
        [Summary("Evaluate a piece of C# code.")]
        [RequireOwner]
        public async Task EvalAsync([Remainder] [Summary("Code to evaluate")] string code)
        {
            // This code has been mostly taken from Nadeko's eval command, available here:
            // https://gitlab.com/Kwoth/nadekobot/-/blob/v4/src/NadekoBot/Modules/Utility/VerboseErrors/EvalCommands.cs
            // Credits to Kwoth for the original code.
            // However, I wish to make my own evaluation method later once I have better grasp of
            // C#. It's mostly here for me to learn C# and better understand things.

            // So, Kwoth, if you end up seeing this message, I thank you a lot, and if this is
            // a bother to you, feel free to contact me: lydia39

            using (Context.Channel.EnterTypingState())
            {
                if (code.StartsWith("```cs"))
                    code = code[5..];
                else if (code.StartsWith("```"))
                    code = code[3..];

                if (code.EndsWith("```"))
                    code = code[..^3];

                var script = CSharpScript.Create(
                    code,
                    ScriptOptions
                        .Default.WithReferences(GetType().Assembly)
                        .WithImports("System", "Elster", "System.Text", "System.Text.Json"),
                    globalsType: typeof(EvalGlobals)
                );

                try
                {
                    var result = await script.RunAsync(
                        new EvalGlobals()
                        {
                            ctx = Context,
                            msg = Context.Message,
                            guild = Context.Guild,
                            channel = Context.Channel,
                            user = Context.User,
                            services = _services
                        }
                    );

                    var output = result.ReturnValue?.ToString();

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        await Context.Channel.SendMessageAsync(
                            output.Truncate(1800).AsCodeBlock("cs")
                        );
                    }
                    else
                    {
                        await Context.Message.Tick();
                    }
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(
                        $"An exception has occurred:\n{ex.Message.Truncate(1800).AsCodeBlock("cs")}"
                    );
                }
            }
        }
    }
}
