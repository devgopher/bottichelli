using System.ComponentModel.DataAnnotations.Schema;

namespace Botticelli.BotData.Entities.Bot.Broadcasting;

[Table("Chat")]
public class Chat
{
    public required string BotId { get; set; }
    public required string ChatId { get; set; }
}