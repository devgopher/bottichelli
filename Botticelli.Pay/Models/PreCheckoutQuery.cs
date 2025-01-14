using Botticelli.Shared.ValueObjects;

namespace Botticelli.Pay.Models;

/// <summary>
/// A pre checkout query after sending an invoice to a user
/// </summary>
public class PreCheckoutQuery
{
    /// <summary>
    ///     Query ID
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     User
    /// </summary>
    public required User From { get; set; }

    /// <summary>
    ///     Currency
    /// </summary>
    public required Currency Currency { get; set; }

    /// <summary>
    ///     Total amount in format: 11.50
    /// </summary>
    public required decimal TotalAmount { get; set; }

    /// <summary>
    ///     A payload
    /// </summary>
    public required string InvoicePayload { get; set; }

    /// <summary>
    ///     Identifier of the shipping option chosen by the user
    /// </summary>
    public string? ShippingOptionId { get; set; }

    /// <summary>
    ///     User-provided order info
    /// </summary>
    public OrderInfo? OrderInfo { get; set; }
}