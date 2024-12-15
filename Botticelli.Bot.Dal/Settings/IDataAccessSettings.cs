namespace Botticelli.Bot.Data.Settings;

/// <summary>
/// Bot data access layer settings
/// </summary>
public interface IDataAccessSettings
{
    public static string Section { get; }
    string ConnectionString { get; set; }
}