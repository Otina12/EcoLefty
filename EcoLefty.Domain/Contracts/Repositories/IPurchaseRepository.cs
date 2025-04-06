using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Domain.Contracts.Repositories;

public interface IPurchaseRepository : IBaseRepository<Purchase, int>
{
    Task CancelAllPurchasesByOfferAsync(int offerId, CancellationToken token);
}
