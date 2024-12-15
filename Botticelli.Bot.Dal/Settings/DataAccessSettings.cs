namespace Botticelli.Bot.Data.Settings;

/// <summary>
/// Bot data access layer settings
/// </summary>
public class DataAccessSettings : IDataAccessSettings
{
    public static string Section => "DataAccess";
    public string? ConnectionString { get; set; }
}