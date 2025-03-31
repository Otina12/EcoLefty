using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class CategoryNotFoundException : NotFoundException
{
    public CategoryNotFoundException(int id) : base($"Category with Id: {id} does not exist.")
    {
    }

    public CategoryNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
