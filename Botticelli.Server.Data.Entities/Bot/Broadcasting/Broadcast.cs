using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botticelli.Server.Data.Entities.Bot.Broadcasting;

[Table("Broadcasts")]
public class Broadcast
{
    [Key] public required string Id { get; set; }

    public required string BotId { get; set; }
    public required string Body { get; set; }
    public DateTime Timestamp { get; set; }

    public BroadcastAttachment[]? Attachments { get; set; }
    // public bool ReceivedSuccessfully { get; set; } = false;
}