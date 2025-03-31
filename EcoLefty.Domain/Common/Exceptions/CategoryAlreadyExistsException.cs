using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class CategoryAlreadyExistsException : AlreadyExistsException
{
    public CategoryAlreadyExistsException(string? name) : base($"Category with Name: {name} already exists.")
    {
    }

    public CategoryAlreadyExistsException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
