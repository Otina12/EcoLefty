using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category, int>, ICategoryRepository
{
    public CategoryRepository(EcoLeftyDbContext context) : base(context)
    {
    }
}
