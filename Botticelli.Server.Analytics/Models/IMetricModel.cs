using System.Runtime.InteropServices;

namespace Botticelli.Server.Analytics.Models;

public interface IMetricModel
{
    string Id { get; set; }
    string? BotId { get; set; }
    DateTime Timestamp { get; set; }
    public string InternalValue { get; set; }
    public Dictionary<string, string> AdditionalParameters { get; set; }
}