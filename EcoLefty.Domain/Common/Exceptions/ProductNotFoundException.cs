using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(int id) : base($"A product with Id: {id} does not exist.")
    {
    }

    public ProductNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
