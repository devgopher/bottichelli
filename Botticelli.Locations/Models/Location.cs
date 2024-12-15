namespace Botticelli.Locations.Models;

/// <summary>
///     Location
/// </summary>
public class Location
{
    public Location()
    {
    }

    public Location(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }

    /// <summary>
    ///     Latitude
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    ///     Longitude
    /// </summary>
    public double Lng { get; set; }
}