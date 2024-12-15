using System;
using System.Linq;
using System.Threading.Tasks;
using Botticelli.Locations.Integration;
using Botticelli.Locations.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Location = Botticelli.Locations.Models.Location;

namespace Botticelli.Locations.Tests.Integration;

[TestFixture]
[TestOf(typeof(OsmLocationProvider))]
public class OsmLocationProviderTest
{
    private readonly ILocationProvider _locationProvider;

    public OsmLocationProviderTest()
    {
        var reverseGeocoder = new ReverseGeocoderMock();
        var forwardGeocoder = new ForwardGeocoderMock();
        var options = Mock.Of<IOptionsSnapshot<LocationsProcessorOptions>>();
        _locationProvider = new OsmLocationProvider(reverseGeocoder, forwardGeocoder, options);
    }

    [Test]
    [TestCase("Test", 1000)]
    public async Task SearchTest(string query, int maxPoints)
    {
        var result = (await _locationProvider.Search(query, maxPoints))?.ToArray();

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Test]
    [TestCase(22, 44)]
    public async Task GetAddressTest(double lat, double lng)
    {
        var result = await _locationProvider.GetAddress(new Location(lat, lng));

        result.Should().NotBeNull();
    }

    [Test]
    [TestCase(51, 0)]
    public async Task GetTimeZoneTest(double lat, double lng)
    {
        var result = await _locationProvider.GetTimeZone(new Location(lat, lng));

        result.Should().NotBeNull();
        result?.BaseUtcOffset.Should().Be(TimeSpan.Zero);
    }
}