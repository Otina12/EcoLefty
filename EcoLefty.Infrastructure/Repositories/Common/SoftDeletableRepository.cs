using EcoLefty.Domain.Common;
using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories.Common;

public class SoftDeletableRepository : ISoftDeletableRepository
{
    private readonly EcoLeftyDbContext _context;

    public SoftDeletableRepository(EcoLeftyDbContext context)
    {
        _context = context;
    }

    public virtual void Deactivate<T>(T entity) where T : SoftDeletableEntity
    {
        var now = DateTime.UtcNow;

        entity.DeletedAtUtc = now;
        entity.UpdatedAtUtc = now;

        _context.Set<T>().Update(entity);
    }

    public virtual void Reactivate<T>(T entity) where T : SoftDeletableEntity
    {
        var now = DateTime.UtcNow;

        entity.DeletedAtUtc = null;
        entity.UpdatedAtUtc = now;

        _context.Set<T>().Update(entity);
    }

    public virtual IQueryable<T> FilterOutDeleted<T>(IEnumerable<T> entities) where T : SoftDeletableEntity
    {
        return entities.AsQueryable().Where(x => x.DeletedAtUtc == null);
    }
}