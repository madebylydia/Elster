namespace Elster.Helpers.Config;

public sealed class RootSettings
{
    public required Bot Bot { get; set; }
}

public sealed class Bot
{
    public required string Token { get; set; }
}
