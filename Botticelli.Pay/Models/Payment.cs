namespace Botticelli.Pay.Models;

public abstract class Payment
{
    /// <summary>
    ///     Currency
    /// </summary>
    public required Currency Currency { get; set; }

    /// <summary>
    ///     Total amount in format: 11.50
    /// </summary>
    public required decimal TotalAmount { get; set; }

    /// <summary>
    ///     Invoice payload
    /// </summary>
    public required string InvoicePayload { get; set; }

    /// <summary>
    ///     Charge id in messenger
    /// </summary>
    public required string MessengerPaymentChargeId { get; set; }

    /// <summary>
    ///     Charge id in a partner system
    /// </summary>
    public required string ProviderPaymentChargeId { get; set; }
}