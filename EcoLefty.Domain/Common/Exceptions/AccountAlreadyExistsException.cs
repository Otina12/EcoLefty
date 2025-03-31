using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class AccountAlreadyExistsException : AlreadyExistsException
{
    public AccountAlreadyExistsException(string email) : base($"An account with Email: {email} already exists.")
    {
    }

    public AccountAlreadyExistsException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
