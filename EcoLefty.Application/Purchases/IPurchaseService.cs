using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Purchases;

public interface IPurchaseService
{
    Task<IEnumerable<Purchase>> GetAllAsync(CancellationToken token = default);
    Task<Purchase> GetByIdAsync(int purchaseId, CancellationToken token = default);
    Task<IEnumerable<Purchase>> GetPurchasesByOfferAsync(int offerId, CancellationToken token = default);
    Task<IEnumerable<Purchase>> GetPurchasesByCustomerAsync(int customerId, CancellationToken token = default);
    Task<bool> CancelPurchaseAsync(int purchaseId, CancellationToken token = default);
    Task<bool> CancelPurchasesByOfferAsync(int offerId, CancellationToken token = default);
    Task<bool> CancelPurchasesByCustomerAsync(int customerId, CancellationToken token = default);
}
