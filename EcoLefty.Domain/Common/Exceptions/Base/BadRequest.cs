namespace EcoLefty.Domain.Common.Exceptions.Base;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {

    }

    public BadRequestException() : base("An error occured while performing this action.")
    {

    }
}
