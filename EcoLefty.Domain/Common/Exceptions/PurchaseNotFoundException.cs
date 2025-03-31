using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public class PurchaseNotFoundException : NotFoundException
{
    public PurchaseNotFoundException(int id) : base($"Purchase with Id: {id} does not exist.")
    {
    }

    public PurchaseNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
