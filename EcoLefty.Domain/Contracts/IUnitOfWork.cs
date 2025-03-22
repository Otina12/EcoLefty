using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Contracts.Repositories.Common;

namespace EcoLefty.Domain.Contracts;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    //EntityEntry<T> Entry<T>(T entity) where T : class;
    //void Attach<T>(T entity) where T : class;
    //void Detach<T>(T entity) where T : class;

    IApplicationUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    ICompanyRepository Companies { get; }
    IOfferRepository Offers { get; }
    IProductRepository Products { get; }

    ISoftDeletableRepository SoftDeleteRepository { get; }
}