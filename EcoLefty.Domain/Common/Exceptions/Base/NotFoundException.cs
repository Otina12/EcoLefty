namespace EcoLefty.Domain.Common.Exceptions.Base;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string? message) : base(message)
    {
    }

    protected NotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}