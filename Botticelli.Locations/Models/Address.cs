using System.Globalization;
using System.Text.Json.Serialization;

namespace Botticelli.Locations.Models;

public class Address
{
    [JsonPropertyName("country")] public string? Country { get; set; }

    [JsonPropertyName("country_code")] public string? CountryCode { get; set; }

    [JsonPropertyName("county")] public string? County { get; set; }

    [JsonPropertyName("house_number")] public string? HouseNumber { get; set; }

    [JsonPropertyName("postcode")] public string? PostCode { get; set; }

    [JsonPropertyName("road")] public string? Road { get; set; }

    [JsonPropertyName("state")] public string? State { get; set; }

    [JsonPropertyName("town")] public string? Town { get; set; }

    [JsonPropertyName("pedestrian")] public string? Pedestrian { get; set; }

    [JsonPropertyName("neighborhood")] public string? Neighborhood { get; set; }

    [JsonPropertyName("hamlet")] public string? Hamlet { get; set; }

    [JsonPropertyName("suburb")] public string? Suburb { get; set; }

    [JsonPropertyName("village")] public string? Village { get; set; }

    [JsonPropertyName("city")] public string? City { get; set; }

    [JsonPropertyName("region")] public string? Region { get; set; }

    [JsonPropertyName("state_district")] public string? District { get; set; }

    [JsonPropertyName("lat")] public double Latitude { get; set; }

    [JsonPropertyName("lon")] public double Longitude { get; set; }

    public string DisplayName { get; set; }

    public override string ToString()
        => ToString(CultureInfo.InvariantCulture);

    public virtual string ToString(CultureInfo culture)
    {
        string result;
        switch (culture.Name.ToLowerInvariant())
        {
            case "ru-ru":
                result = $"{Country}, {PostCode}, {Region}";
                if (State != null) result += $", {State}";
                if (County != null) result += $", {County}";
                if (City != null) result += $", {City}";
                if (Town != null) result += $", {Town}";
                if (Suburb != null) result += $", {Suburb}";
                if (District != null) result += $", {District}";
                if (Village != null) result += $", {Village}";
                if (Hamlet != null) result += $", {Hamlet}";
                if (Neighborhood != null) result += $", {Neighborhood}";
                if (Road != null) result += $", {Road}";
                if (HouseNumber != null) result += $", {HouseNumber}";
                return result;
            default:
                result = $"{Road}, {HouseNumber}";

                if (District != null) result += $", {District}";
                if (City != null) result += $", {City}";
                if (Town != null) result += $", {Town}";
                if (Suburb != null) result += $", {Suburb}";
                if (Village != null) result += $", {Village}";
                if (Hamlet != null) result += $", {Hamlet}";
                if (Neighborhood != null) result += $", {Neighborhood}";
                if (County != null) result += $", {County}";
                if (State != null) result += $", {State}";
                if (Country != null) result += $", {Country}";
                return result;
        }
    }
}