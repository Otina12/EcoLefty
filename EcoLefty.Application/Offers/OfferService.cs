using AutoMapper;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Offers;

public class OfferService : IOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OfferService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OfferDetailsResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var offers = await _unitOfWork.Offers.GetAllAsync(
            trackChanges: false,
            token: token,
            OfferIncludes.Product,
            OfferIncludes.Product_Company,
            OfferIncludes.Product_Categories
        ); // <- real

        return _mapper.Map<IEnumerable<OfferDetailsResponseDto>>(offers);
    }

    public async Task<IEnumerable<OfferDetailsResponseDto>> GetActiveOffersAsync(CancellationToken token = default)
    {
        var now = DateTime.UtcNow;

        var activeOffers = await _unitOfWork.Offers.GetAllWhereAsync(
            o => o.StartDateUtc <= now && o.ExpiryDateUtc >= now,
            trackChanges: false,
            token: token,
            OfferIncludes.Product,
            OfferIncludes.Product_Company,
            OfferIncludes.Product_Categories
        );

        return _mapper.Map<IEnumerable<OfferDetailsResponseDto>>(activeOffers);
    }

    public async Task<OfferDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id);
        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        return _mapper.Map<OfferDetailsResponseDto>(offer);
    }

    public async Task<OfferDetailsResponseDto> CreateAsync(CreateOfferRequestDto createOfferDto, CancellationToken token = default)
    {
        var offer = _mapper.Map<Offer>(createOfferDto);

        await _unitOfWork.Offers.CreateAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OfferDetailsResponseDto>(offer);
    }

    public async Task<OfferDetailsResponseDto> UpdateAsync(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id);
        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        _mapper.Map(updateOfferDto, offer);

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OfferDetailsResponseDto>(offer);
    }

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OfferNotFoundException"></exception>
    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id);
        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        _unitOfWork.Offers.Delete(offer);

        var deleted = await _unitOfWork.SaveChangesAsync();
        return deleted > 0;
    }
}
