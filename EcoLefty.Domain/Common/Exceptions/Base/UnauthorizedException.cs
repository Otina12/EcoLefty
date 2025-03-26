namespace EcoLefty.Domain.Common.Exceptions.Base;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {

    }

    public UnauthorizedException() : base("Not authorized for this action.")
    {

    }
}
