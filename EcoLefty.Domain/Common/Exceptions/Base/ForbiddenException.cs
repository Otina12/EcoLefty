namespace EcoLefty.Domain.Common.Exceptions.Base;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("You are not authorized to perform this action.")
    {
    }

    public ForbiddenException(string? message) : base(message)
    {
    }

    protected ForbiddenException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
