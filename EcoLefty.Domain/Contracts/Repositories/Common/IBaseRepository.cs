using System.Linq.Expressions;

namespace EcoLefty.Domain.Contracts.Repositories.Common;

public interface IBaseRepository<T, TKey> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllWhereAsync(Expression<Func<T, bool>> where, bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    IQueryable<T> GetAllAsQueryable(bool trackChanges = false, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(TKey id, bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    Task<T?> GetOneWhereAsync(Expression<Func<T, bool>> where, bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    Task<bool> ExistsAsync(TKey id, CancellationToken token = default);
    Task CreateAsync(T entity, CancellationToken token = default);
    Task CreateRangeAsync(IEnumerable<T> entities, CancellationToken token = default);
    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    void DeleteAllWhere(Expression<Func<T, bool>> where);
}
