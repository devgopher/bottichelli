using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.Constants;

namespace Botticelli.BotData.Entities.Bot;

[Table("BotData")]
public class BotData
{
    [Key] public required string BotId { get; set; }
    public BotStatus? Status { get; set; }
    public BotType? Type { get; set; }
    public string? BotKey { get; set; }
    public List<BotAdditionalInfo>? AdditionalInfo { get; set; }
}