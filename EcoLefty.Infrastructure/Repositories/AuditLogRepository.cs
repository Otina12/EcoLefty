using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly EcoLeftyDbContext _context;

    public AuditLogRepository(EcoLeftyDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AuditLog>> GetByEntityAsync(string tableName, string entityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
