using System.Globalization;
using Botticelli.Locations.Models;
using Botticelli.Locations.Options;
using GeoTimeZone;
using Mapster;
using Microsoft.Extensions.Options;
using Nominatim.API.Interfaces;
using Nominatim.API.Models;

namespace Botticelli.Locations.Integration;

public class OsmLocationProvider : ILocationProvider
{
    private readonly IForwardGeocoder _forwardGeocoder;
    private readonly IOptionsSnapshot<LocationsProcessorOptions> _options;
    private readonly IReverseGeocoder _reverseGeoCoder;

    public OsmLocationProvider(IReverseGeocoder reverseGeoCoder,
        IForwardGeocoder forwardGeocoder,
        IOptionsSnapshot<LocationsProcessorOptions> options)
    {
        _reverseGeoCoder = reverseGeoCoder;
        _forwardGeocoder = forwardGeocoder;
        _options = options;
    }

    public async Task<Address?> GetAddress(Location location)
        => await InnerGetAddress(location);

    public async Task<string> GetMapLink(Location location) =>
        $"{_options.Value.ApiUrl}/" +
        $"#map={(int)_options.Value.InitialZoom}/" +
        $"{location.Lat.ToString("0.00000", CultureInfo.InvariantCulture)}/" +
        $"{location.Lng.ToString("0.00000", CultureInfo.InvariantCulture)}";

    public async Task<string> GetMapLink(Address address) =>
        $"{_options.Value.ApiUrl}/" +
        $"#map={(int)_options.Value.InitialZoom}/" +
        $"{address.Latitude.ToString("0.00000", CultureInfo.InvariantCulture)}/" +
        $"{address.Longitude.ToString("0.00000", CultureInfo.InvariantCulture)}";

    public async Task<IEnumerable<Address>> Search(string query, int maxPoints)
    {
        var results = (await _forwardGeocoder.Geocode(new ForwardGeocodeRequest
        {
            queryString = query
        })).Select(gr =>
        {
            var address = gr.Address?.Adapt<Address>() ?? new Address();
            address.Longitude = gr.Longitude;
            address.Latitude = gr.Latitude;
            address.DisplayName = gr.DisplayName;

            return address;
        }).ToList();

        return results;
    }

    public Task<TimeZoneInfo?> GetTimeZone(Location location)
    {
        var tz = TimeZoneLookup.GetTimeZone(location.Lat, location.Lng).Result;
        var tzi = TimeZoneInfo.FindSystemTimeZoneById(tz);

        return Task.FromResult(tzi)!;
    }

    private async Task<Address?> InnerGetAddress(Location location)
    {
        var response = await _reverseGeoCoder.ReverseGeocode(new ReverseGeocodeRequest
        {
            Latitude = location.Lat,
            Longitude = location.Lng
        });

        var result = response.Address?.Adapt<Address>();

        return result;
    }
}