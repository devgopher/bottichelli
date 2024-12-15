using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Botticelli.Server.Analytics.Models;

public class MetricModel : IMetricModel, INotifyPropertyChanging, INotifyPropertyChanged
{
    public string Name { get; set; }

    [Key] public string Id { get; set; }

    public string? BotId { get; set; }
    public DateTime Timestamp { get; set; }
    public string InternalValue { get; set; }
    public Dictionary<string, string> AdditionalParameters { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;
}