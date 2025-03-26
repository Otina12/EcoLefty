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
}
