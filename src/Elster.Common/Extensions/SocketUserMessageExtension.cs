using Discord;
using Discord.WebSocket;

namespace Elster.Common.Extensions;

public static class SocketUserMessageExtensions
{
    public static Task Tick(this SocketUserMessage message)
    {
        return message.AddReactionAsync(new Emoji("\u2705"));
    }

    public static Task ReplyCodeBlockAsync(
        this SocketUserMessage message,
        string content,
        string lang = ""
    )
    {
        return message.Channel.SendMessageAsync(content.AsCodeBlock(lang));
    }
}
