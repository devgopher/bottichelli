using System.Threading.Tasks;
using Nominatim.API.Interfaces;
using Nominatim.API.Models;

namespace Botticelli.Locations.Tests;

public class ForwardGeocoderMock : IForwardGeocoder
{
    public async Task<GeocodeResponse[]> Geocode(ForwardGeocodeRequest req) => new GeocodeResponse[]
    {
        new()
        {
            OSMID = 110,
            Latitude = 33,
            Longitude = 22,
            DisplayName = "TestResult",
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
            },
            GeoText = "Test Result"
        }
    };
}