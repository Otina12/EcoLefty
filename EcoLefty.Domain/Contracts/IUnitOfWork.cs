using EcoLefty.Domain.Contracts.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EcoLefty.Domain.Contracts;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken token = default, bool softDeleteEnabled = true);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = default);

    //EntityEntry<T> Entry<T>(T entity) where T : class;
    //void Attach<T>(T entity) where T : class;
    //void Detach<T>(T entity) where T : class;

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