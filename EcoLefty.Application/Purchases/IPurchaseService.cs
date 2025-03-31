using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Purchases;

public interface IPurchaseService
{
    Task<IEnumerable<Purchase>> GetAllAsync(CancellationToken token = default);
    Task<Purchase> GetByIdAsync(int purchaseId, CancellationToken token = default);
    Task<IEnumerable<Purchase>> GetPurchasesByOfferAsync(int offerId, CancellationToken token = default);
    Task<IEnumerable<Purchase>> GetPurchasesByCustomerAsync(string customerId, CancellationToken token = default);
    Task<PurchaseDetailsResponseDto> CreatePurchaseAsync(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token = default);
    Task<bool> CancelPurchaseAsync(int purchaseId, CancellationToken token = default);
    Task<bool> CancelAllPurchasesByOfferAsync(int offerId, CancellationToken token = default);
    Task<bool> CancelAllPurchasesByCustomerAsync(string customerId, CancellationToken token = default);
}
