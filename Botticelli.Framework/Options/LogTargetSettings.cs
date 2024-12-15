namespace Botticelli.Framework.Options;

/// <summary>
///     Remote logs collection settings
/// </summary>
public class LogTargetSettings
{
    /// <summary>
    ///     Target URI
    /// </summary>
    public string? TargetUri { get; set; }

    /// <summary>
    ///     Target filepath
    /// </summary>
    public string? Filepath { get; set; }
}