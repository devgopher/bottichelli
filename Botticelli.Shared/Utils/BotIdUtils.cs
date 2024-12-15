using System.Text.RegularExpressions;

namespace Botticelli.Shared.Utils;

public static partial class BotIdUtils
{
    private static readonly Random Random = new((int)(DateTime.Now.Ticks % int.MaxValue));

    private static byte[] GenerateSalt(int size)
    {
        var salt = new byte[size];

        for (var i = 0; i < size; i++) salt[i] = (byte)(Random.Next() % (byte.MaxValue + 1));

        return salt;
    }

    private static byte[] BitWiseSum(byte[] a1, byte[] a2)
    {
        var min = Math.Min(a1.Length, a2.Length);
        var shortest = a1.Length < a2.Length ? a1 : a2;
        var longest = a1.Length > a2.Length ? a1 : a2;

        for (var i = 0; i < min; ++i) longest[i] |= shortest[i];

        return longest;
    }

    public static string? GenerateShortBotId()
        => ReplaceSymbols().Replace(Convert.ToBase64String(BitWiseSum(Guid.NewGuid().ToByteArray(),
                GenerateSalt(32))), string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);

    [GeneratedRegex("[/+=]")]
    private static partial Regex ReplaceSymbols();
}