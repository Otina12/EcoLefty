using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class ApplicationUserNotFoundException : NotFoundException
{
    public ApplicationUserNotFoundException(int id) : base($"User with Id: {id} does not exist.")
    {
    }
}
