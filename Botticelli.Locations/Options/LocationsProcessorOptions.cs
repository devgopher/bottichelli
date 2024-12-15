namespace Botticelli.Locations.Options;

/// <summary>
///     Location processor options
/// </summary>
public class LocationsProcessorOptions
{
    /// <summary>
    ///     Url
    /// </summary>
    public string ApiUrl { get; set; } = "https://www.openstreetmap.org"; // temporary

    /// <summary>
    ///     Initial zoom for map
    /// </summary>
    public float InitialZoom { get; set; } = 18.0f;
}