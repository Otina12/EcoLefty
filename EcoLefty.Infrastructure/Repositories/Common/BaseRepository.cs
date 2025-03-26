using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcoLefty.Infrastructure.Repositories.Common;

/// <summary>
/// Generic repository implementation providing basic CRUD operations and several methods for additional functionality.
/// Supports different primary key types through the generic parameter <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
public abstract class BaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class where TKey : IEquatable<TKey>
{
    private readonly EcoLeftyDbContext _context;
    protected readonly DbSet<T> dbSet;

    public BaseRepository(EcoLeftyDbContext context)
    {
        _context = context;
        dbSet = _context.Set<T>();
    }

    /// <summary>
    /// Retrieves all entities from the table as a list.
    /// WARNING: The whole table is loaded into the memory. Use this only when needed and be aware of possible outcome.
    /// </summary>
    /// <param name="trackChanges">Indicates whether tracking is enabled for the retrieved entities.</param>
    /// <param name="token">Cancellation token.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>A list of all entities.</returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = dbSet;

        if (!trackChanges)
            query = query.AsNoTracking();

        if (includes != null && includes.Length != 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.ToListAsync(token);
    }

    /// <summary>
    /// Returns an IQueryable to enable further filtering, sorting, and pagination.
    /// More efficient than <see cref="GetAllAsync"/> for scenarios where not all data needs to be loaded immediately.
    /// </summary>
    /// <param name="trackChanges">Indicates whether tracking is enabled for the retrieved entities.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>An <see cref="IQueryable{T}"/> query for further manipulation.</returns>
    public virtual IQueryable<T> GetAllAsQueryable(bool trackChanges = false, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = trackChanges ? dbSet : dbSet.AsNoTracking();

        if (includes != null && includes.Length != 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    /// <summary>
    /// Retrieves all entities matching the given condition as a list. Uses <see cref="GetAllAsQueryable}"/> for better performance.
    /// </summary>
    /// <param name="where">The filter condition.</param>
    /// <param name="trackChanges">Indicates whether tracking is enabled for the retrieved entities.</param>
    /// <param name="token">Cancellation token.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>A list of matching entities.</returns>
    public virtual async Task<IEnumerable<T>> GetAllWhereAsync(Expression<Func<T, bool>> where, bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = trackChanges ? dbSet.Where(where) : dbSet.Where(where).AsNoTracking();

        if (includes != null && includes.Length != 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.ToListAsync(token);
    }

    /// <summary>
    /// Retrieves a single entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="trackChanges">Indicates whether tracking is enabled for the retrieved entity.</param>
    /// <param name="token">Cancellation token.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public virtual async Task<T?> GetByIdAsync(TKey id, bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = trackChanges ? dbSet : dbSet.AsNoTracking();

        if (includes != null && includes.Length != 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id), token);

    }

    /// <summary>
    /// Retrieves a single entity matching the specified condition. Preferable to use with unique, or supposedly unique columns.
    /// </summary>
    /// <param name="where">The filter condition.</param>
    /// <param name="trackChanges">Indicates whether tracking is enabled for the retrieved entity.</param>
    /// <param name="token">Cancellation token.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public virtual async Task<T?> GetOneWhereAsync(Expression<Func<T, bool>> where, bool trackChanges = false, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = trackChanges ? dbSet : dbSet.AsNoTracking();

        if (includes != null && includes.Length != 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.FirstOrDefaultAsync(where, token);
    }

    /// <summary>
    /// Checks if an entity with the specified ID exists, without tracking.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken token = default)
    {
        var entity = await dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id), token);
        return entity is not null;
    }

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="token">Cancellation token.</param>
    public virtual async Task CreateAsync(T entity, CancellationToken token = default)
    {
        await dbSet.AddAsync(entity, token);
    }

    /// <summary>
    /// Adds multiple entities to the database in bulk.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="token">Cancellation token.</param>
    public virtual async Task CreateRangeAsync(IEnumerable<T> entities, CancellationToken token = default)
    {
        await dbSet.AddRangeAsync(entities, token);
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public virtual void Update(T entity)
    {
        dbSet.Update(entity);
    }

    /// <summary>
    /// Deletes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public virtual void Delete(T entity)
    {
        dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Deletes all entities that match the given condition.
    /// </summary>
    /// <param name="where">The filter condition to identify entities for deletion.</param>
    public virtual void DeleteAllWhere(Expression<Func<T, bool>> where)
    {
        IQueryable<T> entitiesToRemove = dbSet.Where(where);
        dbSet.RemoveRange(entitiesToRemove);
    }
}