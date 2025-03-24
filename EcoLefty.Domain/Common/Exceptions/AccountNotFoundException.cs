using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class AccountNotFoundException : NotFoundException
{
    public AccountNotFoundException(string email) : base($"An account with Email: {email} does not exist.")
    {
    }
}
