namespace EcoLefty.Domain.Common.Exceptions;

public class CompanyNotApprovedException : Exception
{
    public CompanyNotApprovedException(int id) : base($"Company with Id: {id} has not been approved by an administator yet.")
    {

    }
}
