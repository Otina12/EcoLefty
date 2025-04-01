using EcoLefty.Application.Purchases.DTOs;

namespace EcoLefty.Application.Purchases;

public interface IPurchaseService
{
    Task<IEnumerable<PurchaseDetailsResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<PurchaseDetailsResponseDto> GetByIdAsync(int purchaseId, CancellationToken token = default);
    Task<IEnumerable<PurchaseDetailsResponseDto>> GetPurchasesByOfferAsync(int offerId, CancellationToken token = default);
    Task<IEnumerable<PurchaseDetailsResponseDto>> GetPurchasesByCustomerAsync(string customerId, CancellationToken token = default);
    Task<PurchaseDetailsResponseDto> CreatePurchaseAsync(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token = default);
    Task<bool> CancelPurchaseAsync(int purchaseId, CancellationToken token = default);
    Task<bool> CancelAllPurchasesByOfferAsync(int offerId, CancellationToken token = default);
    Task<bool> CancelAllPurchasesByCustomerAsync(string customerId, CancellationToken token = default);
}
