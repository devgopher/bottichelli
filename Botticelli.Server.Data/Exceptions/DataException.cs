namespace Botticelli.Server.Data.Exceptions;

public class DataException : Exception
{
    public DataException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}