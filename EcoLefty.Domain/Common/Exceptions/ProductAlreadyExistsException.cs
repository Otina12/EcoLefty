using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class ProductAlreadyExistsException : AlreadyExistsException
{
    public ProductAlreadyExistsException(string name) : base($"Product with Name: {name} already exists at your company.")
    {
    }

    public ProductAlreadyExistsException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
