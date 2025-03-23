namespace EcoLefty.Domain.Common.Exceptions;

public sealed class AccountAlreadyExistsException : Exception
{
    public AccountAlreadyExistsException(string email) : base($"An account with Email: {email} already exists.")
    {
    }
}
