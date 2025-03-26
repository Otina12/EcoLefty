using AutoMapper;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Purchases
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Purchase>> GetAllAsync(CancellationToken token = default)
        {
            return await _unitOfWork.Purchases.GetAllAsync(trackChanges: false, token: token);
        }

        public async Task<Purchase> GetByIdAsync(int purchaseId, CancellationToken token = default)
        {
            var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId, trackChanges: false, token: token);
            if (purchase is null)
            {
                throw new PurchaseNotFoundException(purchaseId);
            }

            return purchase;
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByOfferAsync(int offerId, CancellationToken token = default)
        {
            return await _unitOfWork.Purchases.GetAllWhereAsync(p => p.OfferId == offerId, trackChanges: false, token: token);
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByCustomerAsync(int customerId, CancellationToken token = default)
        {
            return await _unitOfWork.Purchases.GetAllWhereAsync(p => p.CustomerId == customerId, trackChanges: false, token: token);
        }

        public async Task<PurchaseDetailsResponseDto> CreatePurchaseAsync(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token = default)
        {
            throw new NotImplementedException();
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

            if (purchase.PurchaseStatus != PurchaseStatus.Active)
                throw new BadRequestException($"Purchase cannot be cancelled. Its status is: {purchase.PurchaseStatus}");

            var customer = purchase.Customer;
            var company = purchase.Offer.Product.Company;

            if (customer is null)
            {
                throw new Exception($"Associated customer not found for purchase id {purchase.Id}.");
            }

            if (company is null)
            {
                throw new Exception($"Associated company not found for purchase id {purchase.Id}.");
            }

            customer.Balance += purchase.TotalPrice;
            company.Balance -= purchase.TotalPrice;
            purchase.PurchaseStatus = PurchaseStatus.Cancelled;

            _unitOfWork.Purchases.Update(purchase);
            var cancelled = await _unitOfWork.SaveChangesAsync(token);

            return cancelled > 0;
        }

        public async Task<bool> CancelPurchasesByOfferAsync(int offerId, CancellationToken token = default)
        {
            var purchases = await _unitOfWork.Purchases.GetAllWhereAsync(
                p => p.OfferId == offerId,
                trackChanges: true,
                token: token,
                PurchaseIncludes.Customer,
                PurchaseIncludes.Offer_Product_Company);

            foreach (var purchase in purchases)
            {
                if (purchase.PurchaseStatus != PurchaseStatus.Active)
                    continue;

                var customer = purchase.Customer;
                var company = purchase.Offer.Product.Company;

                if (customer is null)
                {
                    throw new Exception($"Associated customer not found for purchase id {purchase.Id}.");
                }

                if (company is null)
                {
                    throw new Exception($"Associated company not found for purchase id {purchase.Id}.");
                }

                customer.Balance += purchase.TotalPrice;
                company.Balance -= purchase.TotalPrice;

                purchase.PurchaseStatus = PurchaseStatus.Cancelled;

                _unitOfWork.Purchases.Update(purchase);
            }

            var cancelled = await _unitOfWork.SaveChangesAsync(token);
            return cancelled > 0;
        }


        public async Task<bool> CancelPurchasesByCustomerAsync(int customerId, CancellationToken token = default)
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

                var customer = purchase.Customer;
                var company = purchase.Offer.Product.Company;

                if (customer is null)
                {
                    throw new Exception($"Associated customer not found for purchase id {purchase.Id}.");
                }

                if (company is null)
                {
                    throw new Exception($"Associated company not found for purchase id {purchase.Id}.");
                }

                customer.Balance += purchase.TotalPrice;
                company.Balance -= purchase.TotalPrice;

                purchase.PurchaseStatus = PurchaseStatus.Cancelled;

                _unitOfWork.Purchases.Update(purchase);
            }

            var cancelled = await _unitOfWork.SaveChangesAsync(token);
            return cancelled > 0;
        }
    }
}