using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product, int>, IProductRepository
{
    public ProductRepository(EcoLeftyDbContext context) : base(context)
    {
    }
}