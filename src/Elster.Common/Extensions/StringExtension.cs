namespace Elster.Common.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string str, int maximumLength, string suffix = "…")
    {
        return str.Length > maximumLength
            ? string.Concat(str.AsSpan(0, maximumLength), suffix)
            : str;
    }

    public static string AsCodeBlock(this string str, string? lang)
    {
        return $"```{lang}\n{str}\n```";
    }
}
