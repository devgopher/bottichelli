using Botticelli.Locations.Models;
using Mapster;
using Nominatim.API.Models;

namespace Botticelli.Locations.Mapping;

public class AddressMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Put your mapping logic here
        config
            .NewConfig<GeocodeResponse, Address>()
            .Map(dest => dest, src => src.Address)
            .Map(dest => dest, src => src)
            .IgnoreNullValues(true)
            .IgnoreNonMapped(true);
    }
}