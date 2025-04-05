using EcoLefty.Application.Offers.DTOs;

namespace EcoLefty.Application.Offers;

public interface IOfferService
{
    Task<IEnumerable<OfferDetailsResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<IEnumerable<OfferDetailsResponseDto>> GetActiveOffersAsync(CancellationToken token = default);
    Task<IEnumerable<OfferDetailsResponseDto>> GetAllOffersOfCompanyAsync(string companyId, CancellationToken token = default);
    Task<IEnumerable<OfferDetailsResponseDto>> GetActiveOffersOfCompanyAsync(string companyId, CancellationToken token = default);
    Task<OfferDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<OfferDetailsResponseDto> CreateAsync(CreateOfferRequestDto createOfferDto, CancellationToken token = default);
    Task<OfferDetailsResponseDto> UpdateAsync(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token = default);
    Task<bool> CancelAsync(int id, CancellationToken token = default);

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OfferNotFoundException"></exception>
    Task<bool> DeleteAsync(int offerId, CancellationToken token = default);
    Task UpdateStatuses(CancellationToken token);
}