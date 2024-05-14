using Discord;
using Discord.Commands;

namespace Elster.Modules.Owner;

public class EvalGlobals
{
    public required ICommandContext ctx;
    public required IMessage msg;
    public required IUser user;
    public required IMessageChannel channel;
    public required IGuild guild;
    public required IServiceProvider services;
}
