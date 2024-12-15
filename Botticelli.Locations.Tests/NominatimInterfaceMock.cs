using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nominatim.API.Interfaces;

namespace Botticelli.Locations.Tests;

public class NominatimInterfaceMock : INominatimWebInterface
{
    public Task<T> GetRequest<T>(string url, Dictionary<string, string> parameters) =>
        throw new NotImplementedException();
}