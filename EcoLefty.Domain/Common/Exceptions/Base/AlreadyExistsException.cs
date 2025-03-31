namespace EcoLefty.Domain.Common.Exceptions.Base;

public abstract class AlreadyExistsException : Exception
{
    protected AlreadyExistsException(string? message) : base(message)
    {
    }

    protected AlreadyExistsException(string? message, Exception? exception) : base(message, exception)
    {
    }
}