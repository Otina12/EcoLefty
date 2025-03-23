using EcoLefty.Application.Offers.DTOs;

namespace EcoLefty.Application.Offers;

public interface IOfferService
{
    Task<IEnumerable<OfferResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<IEnumerable<OfferResponseDto>> GetActiveOffersAsync(CancellationToken token = default);
    Task<OfferResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<OfferResponseDto> CreateAsync(CreateOfferRequestDto createOfferDto, CancellationToken token = default);
    Task<OfferResponseDto> UpdateAsync(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}