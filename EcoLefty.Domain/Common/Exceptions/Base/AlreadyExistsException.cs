namespace EcoLefty.Domain.Common.Exceptions.Base;

public abstract class AlreadyExistsException : Exception
{
    protected AlreadyExistsException(string message) : base(message)
    {
    }
}