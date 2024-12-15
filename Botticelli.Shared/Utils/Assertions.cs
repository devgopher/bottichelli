using System.Diagnostics.CodeAnalysis;

namespace Botticelli.Shared.Utils;

public static class Assertions
{
    
    public static void NotNull([NotNull]this object? input) => ArgumentNullException.ThrowIfNull(input);
    
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? input) => (input is null ? input : Array.Empty<T>())!;
    public static string EmptyIfNull(this string? input) => !string.IsNullOrEmpty(input) ? input : string.Empty;
}