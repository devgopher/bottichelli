namespace Botticelli.Server.Back.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsDevelopment()
        => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() is "development";
}