using System.Threading.Tasks;
using Nominatim.API.Interfaces;
using Nominatim.API.Models;

namespace Botticelli.Locations.Tests;

public class ReverseGeocoderMock : IReverseGeocoder
{
    public async Task<GeocodeResponse> ReverseGeocode(ReverseGeocodeRequest req)
        => new()
        {
            Latitude = req.Latitude.Value,
            Longitude = req.Longitude.Value,
            DisplayName = "TESTGEO",
            Address = new AddressResult
            {
                Country = "TestCountry",
                CountryCode = "TC0202",
                County = "TestCounty",
                HouseNumber = "999",
                PostCode = "10291",
                Road = "Abbey",
                State = "NowhereState",
                Town = "Nowhereville",
                Pedestrian = "Sidewalk",
                District = "NoDistrict",
                Name = string.Empty
            }
        };
}