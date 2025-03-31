using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class CompanyNotFoundException : NotFoundException
{
    public CompanyNotFoundException(string id) : base($"Company with Id: {id} does not exist.")
    {
    }

    public CompanyNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}

