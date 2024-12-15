namespace Botticelli.Shared.Utils;

public static class Assertions
{
    public static void NotNull(this object? input) => ArgumentNullException.ThrowIfNull(input);

    public static void NotNullOrEmpty<T>(this IEnumerable<T> input)
    {
        input.NotNull();

        if (!input.Any())
            throw new Exception($"{nameof(input)} is empty!");
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? input) =>
        (input is null ? input : Array.Empty<T>())!;

    public static string EmptyIfNull(this string? input) => !string.IsNullOrEmpty(input) ? input : string.Empty;
}