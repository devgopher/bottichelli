using System.ComponentModel.DataAnnotations;

namespace Botticelli.Bot.Data.Entities.Bot;

public class BotAdditionalInfo
{
    [Key] public required string BotId { get; set; }
    public required string ItemName { get; set; }
    public string? ItemValue { get; set; }
}