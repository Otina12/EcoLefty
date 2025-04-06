using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class PurchaseRepository : BaseRepository<Purchase, int>, IPurchaseRepository
{
    public PurchaseRepository(EcoLeftyDbContext context) : base(context)
    {
    }

    public async Task CancelAllPurchasesByOfferAsync(int offerId, CancellationToken token)
    {
        var purchases = await GetAllWhereAsync(
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
                throw new ApplicationUserNotFoundException(purchase.CustomerId);

            if (company is null)
                throw new CompanyNotFoundException(purchase.Offer.Product.CompanyId);

            customer.Balance += purchase.TotalPrice;
            company.Balance -= purchase.TotalPrice;
            purchase.PurchaseStatus = PurchaseStatus.Cancelled;

            Update(purchase);
        }
    }
}
