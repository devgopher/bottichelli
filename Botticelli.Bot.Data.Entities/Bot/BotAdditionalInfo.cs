using System.ComponentModel.DataAnnotations;

namespace Botticelli.BotData.Entities.Bot;

public class BotAdditionalInfo
{
    [Key] public required string BotId { get; set; }
    public required string ItemName { get; set; }
    public string? ItemValue { get; set; }
}