using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class ProductAlreadyExistsException : AlreadyExistsException
{
    public ProductAlreadyExistsException(string name) : base($"Category with Name: {name} already exists at your company.")
    {
    }
}
