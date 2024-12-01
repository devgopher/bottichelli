using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botticelli.Server.Data.Entities.Bot.Broadcasting;

[Table("BroadcastAttachments")]
public class BroadcastAttachment
{
    [Key]
    public Guid Id { get; set; }

    public MediaType MediaType { get; set; }
    public required string Filename { get; set; }
    public required byte[] Content { get; set; }
}