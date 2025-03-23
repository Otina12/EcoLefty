using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

namespace EcoLefty.Infrastructure.Repositories.Common;

/// <summary>
/// Responsible for generating audit logs based on EF Core change tracker entries.
/// </summary>
internal class AuditLogWriter
{
    private readonly IUserContext _userContext;
    private static string? _userId;

    AuditLogWriter(IUserContext userContext)
    {
        _userContext = userContext;
        _userId = _userContext.UserId;
    }

    /// <summary>
    /// Handles audit logging for all tracked entity changes in the given context.
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    /// <param name="userId">The ID of the user performing the operation (optional).</param>
    public static void HandleAuditLogging(EcoLeftyDbContext context)
    {
        var now = DateTime.UtcNow;
        var entries = context.ChangeTracker.Entries();
        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            {
                continue;
            }

            var action = GetActionType(entry.State);
            if (action == null)
            {
                continue;
            }

            var log = new AuditLog
            {
                UserId = _userId ?? string.Empty,
                ActionType = action.Value,
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.State == EntityState.Added ? "N/A" : entry.CurrentValues["Id"]?.ToString() ?? "N/A",
                Timestamp = now,
                Changes = action.Value switch
                {
                    ActionType.Created => SerializeValues(entry.CurrentValues.Properties, entry.CurrentValues),
                    ActionType.Updated => SerializeDifferences(entry),
                    ActionType.Deleted => SerializeValues(entry.OriginalValues.Properties, entry.OriginalValues),
                    _ => "{}"
                }
            };

            auditLogs.Add(log);
        }

        if (auditLogs.Count != 0)
        {
            context.AuditLogs.AddRange(auditLogs);
        }
    }

    private static ActionType? GetActionType(EntityState state)
    {
        return state switch
        {
            EntityState.Added => ActionType.Created,
            EntityState.Modified => ActionType.Updated,
            EntityState.Deleted => ActionType.Deleted,
            _ => null
        };
    }

    private static string SerializeValues(IEnumerable<IProperty> properties, PropertyValues values)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var prop in properties)
        {
            dict[prop.Name] = values[prop.Name];
        }

        return JsonSerializer.Serialize(dict);
    }

    private static string SerializeDifferences(EntityEntry entry)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var prop in entry.OriginalValues.Properties)
        {
            var original = entry.OriginalValues[prop.Name];
            var current = entry.CurrentValues[prop.Name];
            if (!Equals(original, current))
            {
                dict[prop.Name] = new { Before = original, After = current };
            }
        }

        return JsonSerializer.Serialize(dict);
    }
}
