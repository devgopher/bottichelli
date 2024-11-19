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
}