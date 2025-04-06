using EcoLefty.Domain.Contracts.Repositories;

namespace EcoLefty.Domain.Contracts;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken token = default, bool softDeleteEnabled = true);
    IAccountRepository Accounts { get; }
    IApplicationUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    ICompanyRepository Companies { get; }
    IOfferRepository Offers { get; }
    IProductRepository Products { get; }
    IPurchaseRepository Purchases { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    ICurrentUserContext CurrentUserContext { get; }
}