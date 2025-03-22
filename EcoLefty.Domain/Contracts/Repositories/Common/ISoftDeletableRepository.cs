using EcoLefty.Domain.Common;

namespace EcoLefty.Domain.Contracts.Repositories.Common;

public interface ISoftDeletableRepository
{
    void Deactivate<T>(T entity) where T : SoftDeletableEntity;

    void Reactivate<T>(T entity) where T : SoftDeletableEntity;

    IQueryable<T> FilterOutDeleted<T>(IEnumerable<T> entities) where T : SoftDeletableEntity;
}
