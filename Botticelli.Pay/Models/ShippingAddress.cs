namespace Botticelli.Pay.Models;

/// <summary>
///     User's shipping address
/// </summary>
public class ShippingAddress
{
    /// <summary>
    ///     ISO 3166-1 alpha-2 country code
    /// </summary>
    public required string CountryCode { get; set; }

    /// <summary>
    ///     State
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    ///     City
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    ///     Street 1
    /// </summary>
    public required string StreetLine1 { get; set; }

    /// <summary>
    ///     Street 2
    /// </summary>
    public required string StreetLine2 { get; set; }

    /// <summary>
    ///     Postal code
    /// </summary>
    public required string PostCode { get; set; }
}