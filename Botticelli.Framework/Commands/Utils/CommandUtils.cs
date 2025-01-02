using System.Text.RegularExpressions;

namespace Botticelli.Framework.Commands.Utils;

public static class CommandUtils
{
    public static Regex SimpleCommandRegex => new("\\/([a-zA-Z0-9]*)$");
    public static Regex ArgsCommandRegex => new("\\/([a-zA-Z0-9]*) (.*)");
    
    public static string GetArguments(this string? body)
    {
        if (body is null) 
            return string.Empty;
        
        var match = ArgsCommandRegex.Matches(body)
                        .FirstOrDefault();

        return match == null ? string.Empty : match.Groups[2].Value;
    }

    public static string[] GetArguments(this string? body, char separator) => GetArguments(body).Split(separator);
}