namespace Botticelli.Pay.Models;

/// <summary>
///     Successful payment entity
/// </summary>
public class SuccessfulPayment : Payment
{
    /// <summary>
    ///     Subscription expiration time (recurring payments only)
    /// </summary>
    public DateTime? SubscriptionExpirationDate { get; set; }

    /// <summary>
    ///     Is recurring payment or not
    /// </summary>
    public bool? IsRecurring { get; set; }

    /// <summary>
    ///     Is a first recurring payment
    /// </summary>
    public bool? IsFirstRecurring { get; set; }

    /// <summary>
    ///     Identifier of the shipping option chosen by the user
    /// </summary>
    public string? ShippingOptionId { get; set; }

    /// <summary>
    ///     Additional order information, provided by user
    /// </summary>
    public OrderInfo? OrderInfo { get; set; }
}