namespace EcoLefty.Domain.Common.Exceptions.Base;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Unauthorized to perform this action.")
    {
    }

    public UnauthorizedException(string? message) : base(message)
    {
    }

    protected UnauthorizedException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
