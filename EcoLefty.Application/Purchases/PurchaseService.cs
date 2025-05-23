﻿using AutoMapper;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Purchases;

public class PurchaseService : IPurchaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PurchaseDetailsResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var purchases = await _unitOfWork.Purchases.GetAllAsync(trackChanges: false, token: token);
        return _mapper.Map<IEnumerable<PurchaseDetailsResponseDto>>(purchases);
    }

    public async Task<PurchaseDetailsResponseDto> GetByIdAsync(int purchaseId, CancellationToken token = default)
    {
        var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId, trackChanges: false, token: token,
            PurchaseIncludes.Customer_Account,
            PurchaseIncludes.Offer);
        if (purchase is null)
        {
            throw new PurchaseNotFoundException(purchaseId);
        }

        return _mapper.Map<PurchaseDetailsResponseDto>(purchase);
    }

    public async Task<IEnumerable<PurchaseDetailsResponseDto>> GetPurchasesByOfferAsync(int offerId, CancellationToken token = default)
    {
        var purchases = await _unitOfWork.Purchases.GetAllWhereAsync(p => p.OfferId == offerId, trackChanges: false, token: token);
        return _mapper.Map<IEnumerable<PurchaseDetailsResponseDto>>(purchases);
    }

    public async Task<IEnumerable<PurchaseDetailsResponseDto>> GetPurchasesByCustomerAsync(string customerId, CancellationToken token = default)
    {
        var purchases = await _unitOfWork.Purchases.GetAllWhereAsync(p => p.CustomerId == customerId, trackChanges: false, token: token);
        return _mapper.Map<IEnumerable<PurchaseDetailsResponseDto>>(purchases);
    }

    public async Task<PurchaseDetailsResponseDto> CreatePurchaseAsync(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token = default)
    {
        var currentUserId = _unitOfWork.CurrentUserContext.UserId;
        if (currentUserId is null)
            throw new UnauthorizedException();

        var user = await _unitOfWork.Users.GetByIdAsync(currentUserId, true, token);
        if (user is null)
            throw new UnauthorizedException();

        var offer = await _unitOfWork.Offers.GetByIdAsync(createPurchaseDto.OfferId, true, token, OfferIncludes.Product_Company);
        if (offer is null)
            throw new OfferNotFoundException(createPurchaseDto.OfferId);

        if (offer.QuantityAvailable < createPurchaseDto.Quantity)
            throw new BadRequestException("Not enough quantity available in the specified offer.");

        var purchasePrice = createPurchaseDto.Quantity * offer.UnitPrice;

        if (purchasePrice > user.Balance)
            throw new BadRequestException("Not enough balance.");

        var company = offer.Product.Company;
        user.Balance -= purchasePrice;
        company.Balance += purchasePrice;
        offer.QuantityAvailable -= createPurchaseDto.Quantity;

        var purchase = _mapper.Map<Purchase>(createPurchaseDto);
        purchase.TotalPrice = purchasePrice;
        purchase.PurchaseDateUtc = DateTime.UtcNow;
        purchase.CustomerId = currentUserId;

        await _unitOfWork.Purchases.CreateAsync(purchase, token);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PurchaseDetailsResponseDto>(purchase);
    }

    public async Task<bool> CancelPurchaseAsync(int purchaseId, CancellationToken token = default)
    {
        var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId,
            trackChanges: true,
            token: token,
            PurchaseIncludes.Customer,
            PurchaseIncludes.Offer_Product_Company);

        if (purchase is null)
            throw new PurchaseNotFoundException(purchaseId);

        if (purchase.CustomerId != _unitOfWork.CurrentUserContext.UserId)
            throw new ForbiddenException();

        if (purchase.PurchaseStatus != PurchaseStatus.Active)
            throw new BadRequestException($"Purchase cannot be cancelled. Its status is: {purchase.PurchaseStatus}");

        var customer = purchase.Customer;
        var company = purchase.Offer.Product.Company;
        var offer = purchase.Offer;

        if (customer is null)
            throw new ApplicationUserNotFoundException(purchase.CustomerId);

        if (company is null)
            throw new CompanyNotFoundException(purchase.Offer.Product.CompanyId);

        if (offer is null)
            throw new OfferNotFoundException(purchase.OfferId);

        if (DateTime.UtcNow - purchase.PurchaseDateUtc > TimeSpan.FromMinutes(5))
            throw new InvalidOperationException("Purchase cancellations are only permitted within the first 5 minutes following the request.");

        customer.Balance += purchase.TotalPrice;
        company.Balance -= purchase.TotalPrice;
        purchase.PurchaseStatus = PurchaseStatus.Cancelled;
        offer.QuantityAvailable += purchase.Quantity;

        _unitOfWork.Purchases.Update(purchase);
        var cancelled = await _unitOfWork.SaveChangesAsync(token);

        return cancelled > 0;
    }

    /// <summary>
    /// Only use when cancelling an offer.
    /// </summary>
    /// <param name="offerId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OfferNotFoundException"></exception>
    public async Task<bool> CancelAllPurchasesByOfferAsync(int offerId, CancellationToken token = default)
    {
        var offerExists = await _unitOfWork.Offers.ExistsAsync(offerId, token: token);
        if (!offerExists)
            throw new OfferNotFoundException(offerId);

        await _unitOfWork.Purchases.CancelAllPurchasesByOfferAsync(offerId, token);

        var cancelled = await _unitOfWork.SaveChangesAsync(token);
        return cancelled > 0;
    }

    public async Task<bool> CancelAllPurchasesByCustomerAsync(string customerId, CancellationToken token = default)
    {
        var purchases = await _unitOfWork.Purchases.GetAllWhereAsync(
            p => p.CustomerId == customerId,
            trackChanges: true,
            token: token,
            PurchaseIncludes.Customer,
            PurchaseIncludes.Offer_Product_Company);

        foreach (var purchase in purchases)
        {
            if (purchase.PurchaseStatus != PurchaseStatus.Active)
                continue;

            var offer = purchase.Offer;
            var customer = purchase.Customer;
            var company = purchase.Offer.Product.Company;

            if (offer is null)
                throw new OfferNotFoundException(purchase.OfferId);

            if (customer is null)
                throw new ApplicationUserNotFoundException(purchase.CustomerId);

            if (company is null)
                throw new CompanyNotFoundException(purchase.Offer.Product.CompanyId);

            purchase.PurchaseStatus = PurchaseStatus.Cancelled;

            offer.QuantityAvailable += purchase.Quantity;

            customer.Balance += purchase.TotalPrice;
            company.Balance -= purchase.TotalPrice;

            _unitOfWork.Purchases.Update(purchase);
        }

        var cancelled = await _unitOfWork.SaveChangesAsync(token);
        return cancelled > 0;
    }
}