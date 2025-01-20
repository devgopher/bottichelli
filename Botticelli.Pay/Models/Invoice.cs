using Telegram.Bot.Types.Payments;

namespace Botticelli.Pay.Models;

/// <summary>
///     An invoice entity
/// </summary>
public class Invoice
{
    /// <summary>
    ///     Product title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Description
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    ///     Payload
    /// </summary>
    public required string Payload { get; set; }
    
    /// <summary>
    ///     Additional parameters for invoice
    /// </summary>
    public IEnumerable<string>? AdditionalParameters { get; set; }

    /// <summary>
    ///     Currency
    /// </summary>
    public required Currency Currency { get; set; }

    /// <summary>
    ///     Prices item-by-item
    /// </summary>
    public required List<Price> Prices { get; set; }

    /// <summary>
    /// Payment provider token
    /// </summary>
    public required string ProviderToken{ get; set; }
    
    /// <summary>
    /// Payment provider data
    /// </summary>
    public string? ProviderData { get; set; }
    
    /// <summary>
    ///     Total amount in format: 11.50
    /// </summary>
    public decimal TotalAmount => Prices?.Sum(x => x.Amount) ?? 0;
}