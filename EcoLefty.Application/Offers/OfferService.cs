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

    public async Task<IEnumerable<OfferResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var offers = await _unitOfWork.Offers.GetAllAsync(trackChanges: false, token: token, OfferIncludeExpressions.Products);

        // TODO. ThenInclude Company and get companydto from productdto
        return _mapper.Map<IEnumerable<OfferResponseDto>>(offers);
    }

    public async Task<IEnumerable<OfferResponseDto>> GetActiveOffersAsync(CancellationToken token = default)
    {
        var now = DateTime.UtcNow;
        var activeOffers = await _unitOfWork.Offers
            .GetAllWhereAsync(
                o => o.StartDateUtc <= now && o.ExpiryDateUtc >= now,
                trackChanges: false,
                token: token,
                OfferIncludeExpressions.Products
            );

        return _mapper.Map<IEnumerable<OfferResponseDto>>(activeOffers);
    }

    public async Task<OfferResponseDto> GetByIdAsync(int id, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id);
        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        return _mapper.Map<OfferResponseDto>(offer);
    }

    public async Task<OfferResponseDto> CreateAsync(CreateOfferRequestDto createOfferDto, CancellationToken token = default)
    {
        var offer = _mapper.Map<Offer>(createOfferDto);

        await _unitOfWork.Offers.CreateAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OfferResponseDto>(offer);
    }

    public async Task<OfferResponseDto> UpdateAsync(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id);
        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        _mapper.Map(updateOfferDto, offer);

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OfferResponseDto>(offer);
    }

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
