namespace Botticelli.Talks.Exceptions;

public class BotticelliTalksException : Exception
{
    public BotticelliTalksException(string message, Exception? inner = default) : base(message, inner)
    {
    }
}