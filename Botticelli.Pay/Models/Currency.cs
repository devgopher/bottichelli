using System.Text.Json.Serialization;

namespace Botticelli.Pay.Models;

public class Currency
{
    [JsonIgnore]
    public string Iso { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("demonym")]
    public required string Demonym { get; set; }

    [JsonPropertyName("majorSingle")]
    public required string MajorSingle { get; set; }

    [JsonPropertyName("majorPlural")]
    public required string MajorPlural { get; set; }

    [JsonPropertyName("ISOnum")]
    public int? IsOnum { get; set; }

    [JsonPropertyName("symbol")]
    public required string Symbol { get; set; }

    [JsonPropertyName("symbolNative")]
    public required string SymbolNative { get; set; }

    [JsonPropertyName("minorSingle")]
    public required string MinorSingle { get; set; }

    [JsonPropertyName("minorPlural")]
    public required string MinorPlural { get; set; }

    [JsonPropertyName("ISOdigits")]
    public int? IsOdigits { get; set; }

    [JsonPropertyName("decimals")]
    public int? Decimals { get; set; }

    [JsonPropertyName("numToBasic")]
    public int? NumToBasic { get; set; }
}
