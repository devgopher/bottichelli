namespace Botticelli.Shared.Utils;

public static class Assertions
{
    public static void NotNull(this object? input) => ArgumentNullException.ThrowIfNull(input);
}