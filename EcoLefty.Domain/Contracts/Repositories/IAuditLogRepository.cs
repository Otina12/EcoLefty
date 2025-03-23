using EcoLefty.Domain.Entities;

namespace EcoLefty.Domain.Contracts.Repositories;

public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string tableName, string entityId);
}
