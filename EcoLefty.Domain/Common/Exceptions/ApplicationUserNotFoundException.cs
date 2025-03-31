using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class ApplicationUserNotFoundException : NotFoundException
{
    public ApplicationUserNotFoundException(string? id) : base($"User with Id: {id} does not exist.")
    {
    }

    public ApplicationUserNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
