namespace EcoLefty.Domain.Common.Exceptions;

public class CompanyNotApprovedException : Exception
{
    public CompanyNotApprovedException(string? id) : base($"Company with Id: {id} has not been approved by an administator.")
    {
    }

    public CompanyNotApprovedException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
