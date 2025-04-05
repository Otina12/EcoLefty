using AutoMapper;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Purchases;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Offers;

public class OfferService : IOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPurchaseService _purchaseService;
    private readonly IMapper _mapper;

    public OfferService(IUnitOfWork unitOfWork, IMapper mapper, IPurchaseService purchaseService)
    {
        _unitOfWork = unitOfWork;
        _purchaseService = purchaseService;
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
            o => now >= o.StartDateUtc && now <= o.ExpiryDateUtc && DateTime.UtcNow >= o.StartDateUtc,
            trackChanges: false,
            token: token,
            OfferIncludes.Product,
            OfferIncludes.Product_Company,
            OfferIncludes.Product_Categories
        );

        return _mapper.Map<IEnumerable<OfferDetailsResponseDto>>(activeOffers);
    }
    public async Task<IEnumerable<OfferDetailsResponseDto>> GetAllOffersOfCompanyAsync(string companyId, CancellationToken token = default)
    {
        var offers = await _unitOfWork.Offers.GetAllWhereAsync(
            o => o.Product.CompanyId == companyId,
            trackChanges: false,
            token: token,
            OfferIncludes.Product,
            OfferIncludes.Product_Company,
            OfferIncludes.Product_Categories
        ); // <- real

        return _mapper.Map<IEnumerable<OfferDetailsResponseDto>>(offers);
    }

    public async Task<IEnumerable<OfferDetailsResponseDto>> GetActiveOffersOfCompanyAsync(string companyId, CancellationToken token = default)
    {
        var now = DateTime.UtcNow;

        var activeOffers = await _unitOfWork.Offers.GetAllWhereAsync(
            o => o.Product.CompanyId == companyId && now >= o.StartDateUtc && now <= o.ExpiryDateUtc,
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
        var offer = await _unitOfWork.Offers.GetByIdAsync(
            id,
            trackChanges: false,
            token: token,
            OfferIncludes.Product,
            OfferIncludes.Product_Company,
            OfferIncludes.Product_Categories);

        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        return _mapper.Map<OfferDetailsResponseDto>(offer);
    }

    public async Task<OfferDetailsResponseDto> CreateAsync(CreateOfferRequestDto createOfferDto, CancellationToken token = default)
    {
        var currentUserId = _unitOfWork.CurrentUserContext.UserId;

        if (currentUserId is null)
            throw new UnauthorizedException();

        var product = await _unitOfWork.Products.GetByIdAsync(createOfferDto.ProductId, false, token);
        var company = await _unitOfWork.Companies.GetByIdAsync(currentUserId, false, token);

        if (product is null)
            throw new ProductNotFoundException(createOfferDto.ProductId);

        if (company is null)
            throw new CompanyNotFoundException($"Company attached to account with Id: {currentUserId} was not found.");

        if (!company.IsApproved)
            throw new CompanyNotApprovedException(company.Id);

        if (company.Id != product.CompanyId)
            throw new Exception($"Product with Id: {product.Id} doesn't belong to company with Id: {company.Id}.");

        var offer = _mapper.Map<Offer>(createOfferDto);
        offer.QuantityAvailable = offer.TotalQuantity;

        await _unitOfWork.Offers.CreateAsync(offer, token);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<OfferDetailsResponseDto>(offer);
    }

    public async Task<OfferDetailsResponseDto> UpdateAsync(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id, trackChanges: true, token, OfferIncludes.Product);
        if (offer is null)
        {
            throw new OfferNotFoundException(id);
        }

        if (offer.Product.CompanyId != _unitOfWork.CurrentUserContext.UserId)
        {
            throw new ForbiddenException();
        }

        _mapper.Map(updateOfferDto, offer);

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<OfferDetailsResponseDto>(offer);
    }

    public async Task<bool> CancelAsync(int id, CancellationToken token = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id, trackChanges: false, token: token, OfferIncludes.Product_Company);
        if (offer is null)
            throw new OfferNotFoundException(id);

        var currentUserId = _unitOfWork.CurrentUserContext.UserId;
        if (currentUserId is null)
            throw new UnauthorizedException();

        if (currentUserId != offer.Product.CompanyId)
            throw new ForbiddenException();

        var company = await _unitOfWork.Companies.GetByIdAsync(currentUserId, false, token);

        if (company is null)
            throw new CompanyNotFoundException($"Company attached to account with Id: {currentUserId} was not found");

        if (DateTime.UtcNow - offer.CreatedAtUtc > TimeSpan.FromMinutes(10))
            throw new InvalidOperationException("Offer cancellations are only permitted within the first 10 minutes following the request.");

        // cancel all purchases and update balances
        await _purchaseService.CancelAllPurchasesByOfferAsync(offer.Id, token);

        _unitOfWork.Offers.Delete(offer);

        var updated = await _unitOfWork.SaveChangesAsync(token);
        return updated > 0;
    }

    public async Task<bool> DeleteAsync(int offerId, CancellationToken token = default) // This is different from cancelling. Balances are not updated. Only the offer is deleted (soft).
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(offerId, true, token, OfferIncludes.Product);
        if (offer is null)
        {
            throw new OfferNotFoundException(offerId);
        }

        // check if user is an admin OR the company that created the offer
        bool isAdmin = _unitOfWork.CurrentUserContext.IsInRole("Admin");
        bool isOwner = offer.Product.CompanyId == _unitOfWork.CurrentUserContext.UserId;

        if (!isAdmin && !isOwner)
        {
            throw new ForbiddenException();
        }

        _unitOfWork.Offers.Delete(offer);

        var deleted = await _unitOfWork.SaveChangesAsync(token);
        return deleted > 0;
    }

    public async Task UpdateStatuses(CancellationToken token)
    {
        var offersQuery = _unitOfWork.Offers.GetAllAsQueryable(true);

        foreach (var offer in offersQuery)
        {
            if (offer.StartDateUtc > DateTime.UtcNow)
            {
                offer.OfferStatus = OfferStatus.Incoming;
            }
            else if (offer.ExpiryDateUtc <= DateTime.UtcNow)
            {
                offer.OfferStatus = OfferStatus.Archived;
            }
        }

        await _unitOfWork.SaveChangesAsync(token);
    }
}
