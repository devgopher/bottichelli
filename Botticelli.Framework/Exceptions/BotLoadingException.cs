namespace Botticelli.Framework.Exceptions;

public class BotLoadingException : Exception
{
    public BotLoadingException(string message, Exception? inner = default) : base(message, inner)
    {
    }
}