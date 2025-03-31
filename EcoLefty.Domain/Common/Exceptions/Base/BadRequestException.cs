namespace EcoLefty.Domain.Common.Exceptions.Base;

public class BadRequestException : Exception
{
    public BadRequestException() : base("An error occured while performing this action.")
    {
    }

    public BadRequestException(string? message) : base(message)
    {
    }

    protected BadRequestException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
