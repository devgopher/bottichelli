﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.Constants;

namespace Botticelli.Server.Data.Entities.Bot;

[Table("BotInfo")]
public class BotInfo
{
    [Key] public required string BotId { get; set; }

    public required string BotName { get; set; }
    public DateTime? LastKeepAlive { get; set; }
    public BotStatus? Status { get; set; }
    public BotType? Type { get; set; }
    public string? BotKey { get; set; } 
    public BotAdditionalInfo AdditionalInfo { get; set; }
}