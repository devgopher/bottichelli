namespace Botticelli.Shared.ValueObjects;

public class Poll
{
    public enum PollType
    {
        Quiz,
        Regular
    }
    
    public string? Id { get; set; }
    public string? Question { get; set; }
    public IEnumerable<(string option, int votersCount)>? Variants { get; set; }
    public int? CorrectAnswerId { get; set; }
    public bool IsAnonymous { get; set; }
    public PollType Type { get; set; }
}