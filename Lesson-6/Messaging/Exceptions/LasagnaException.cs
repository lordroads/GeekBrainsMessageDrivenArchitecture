namespace Messaging.Exceptions;

public class LasagnaException : Exception
{
    public LasagnaException(string? message) : base(message)
    {
    }
}