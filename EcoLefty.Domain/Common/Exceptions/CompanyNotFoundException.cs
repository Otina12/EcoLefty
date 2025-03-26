using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class CompanyNotFoundException : NotFoundException
{
    public CompanyNotFoundException(int id) : base($"Company with Id: {id} does not exist.")
    {
    }

    public CompanyNotFoundException(string message) : base(message)
    {
    }
}

