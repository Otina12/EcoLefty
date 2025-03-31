using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class AccountNotFoundException : NotFoundException
{
    public AccountNotFoundException() : base($"An account with provided details does not exist.")
    {
    }
    public AccountNotFoundException(string? email) : base($"An account with Email: {email} does not exist.")
    {
    }

    public AccountNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
