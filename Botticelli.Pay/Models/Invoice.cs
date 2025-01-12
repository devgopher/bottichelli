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
    public string? Description { get; set; }

    /// <summary>
    ///     Additional parameters for invoice
    /// </summary>
    public IEnumerable<string>? AdditionalParameters { get; set; }

    /// <summary>
    ///     Currency
    /// </summary>
    public required Currency Currency { get; set; }

    /// <summary>
    ///     Total amount in format: 11.50
    /// </summary>
    public required decimal TotalAmount { get; set; }
}