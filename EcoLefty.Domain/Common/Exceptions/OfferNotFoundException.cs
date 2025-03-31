using EcoLefty.Domain.Common.Exceptions.Base;

namespace EcoLefty.Domain.Common.Exceptions;

public sealed class OfferNotFoundException : NotFoundException
{
    public OfferNotFoundException(int id) : base($"An offer with Id: {id} does not exist.")
    {
    }

    public OfferNotFoundException(string? message, Exception? exception) : base(message, exception)
    {
    }
}
