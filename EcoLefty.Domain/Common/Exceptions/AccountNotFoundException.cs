using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class AccountNotFoundException : NotFoundException
{
    public AccountNotFoundException(string id) : base($"An account with Id: {id} does not exist.")
    {
    }
}
