namespace Botticelli.Pay.Models;

/// <summary>
///     User-provided order info
/// </summary>
public class OrderInfo
{
    /// <summary>
    ///     User name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     Shipping address
    /// </summary>
    public ShippingAddress? ShippingAddress { get; set; }
}