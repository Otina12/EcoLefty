using EcoLefty.Domain.Common;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

namespace EcoLefty.Infrastructure;

/// <summary>
/// Responsible for handling soft deletion and generating audit logs based on EF Core change tracker entries.
/// </summary>
internal static class SoftDeleteHelper
{
    /// <summary>
    /// Processes all tracked entity changes - applies soft delete logic and creates audit logs.
    /// </summary>
    /// <param name="context">Database context with tracked entities</param>
    /// <param name="userContext">Current user context to identify who made the changes</param>
    /// <param name="timestamp">Timestamp when changes were made</param>
    public static void ProcessChanges(EcoLeftyDbContext context, IUserContext userContext, DateTime timestamp)
    {
        var entityEntries = context.ChangeTracker.Entries();

        var logs = CreateAuditLogs(entityEntries, userContext.UserId, timestamp);
        context.AddRange(logs);

        ApplySoftDeleteAndTimestamps(entityEntries, timestamp);
    }

    /// <summary>
    /// Applies soft delete pattern and updates timestamps for all tracked entity changes.
    /// </summary>
    /// <param name="entityEntries">All tracked entities</param>
    /// <param name="timestamp">Timestamp of update</param>
    private static void ApplySoftDeleteAndTimestamps(IEnumerable<EntityEntry> entityEntries, DateTime timestamp)
    {
        foreach (var entry in entityEntries)
        {
            if (entry.Entity is not SoftDeletableEntity softDeletable)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    softDeletable.CreatedAtUtc = timestamp;
                    break;

                case EntityState.Modified:
                    softDeletable.UpdatedAtUtc = timestamp;
                    break;

                case EntityState.Deleted:
                    softDeletable.DeletedAtUtc = timestamp;
                    entry.State = EntityState.Modified;
                    CascadeSoftDelete(entry, timestamp, new HashSet<object>());
                    break;
            }
        }
    }

    /// <summary>
    /// Recursively soft-deletes related entities that implement SoftDeletableEntity.
    /// A visited set is used to avoid cycles in the object graph.
    /// </summary>
    private static void CascadeSoftDelete(EntityEntry parentEntry, DateTime timestamp, HashSet<object> visited)
    {
        if (visited.Contains(parentEntry.Entity))
        {
            return;
        }

        visited.Add(parentEntry.Entity);

        foreach (var navigation in parentEntry.Navigations)
        {
            if (!navigation.IsLoaded)
            {
                navigation.Load();
            }

            if (navigation.Metadata.IsCollection)
            {
                if (navigation.CurrentValue is IEnumerable<object> children)
                {
                    foreach (var child in children)
                    {
                        if (child is SoftDeletableEntity softChild && softChild.DeletedAtUtc == null)
                        {
                            softChild.DeletedAtUtc = timestamp;
                            var childEntry = parentEntry.Context.Entry(child);

                            if (childEntry.State != EntityState.Deleted)
                            {
                                childEntry.State = EntityState.Modified;
                                CascadeSoftDelete(childEntry, timestamp, visited);
                            }
                        }
                    }
                }
            }
            else
            {
                var child = navigation.CurrentValue;
                if (child is SoftDeletableEntity softChild && softChild.DeletedAtUtc == null)
                {
                    softChild.DeletedAtUtc = timestamp;
                    var childEntry = parentEntry.Context.Entry(child);
                    if (childEntry.State != EntityState.Deleted)
                    {
                        childEntry.State = EntityState.Modified;
                        CascadeSoftDelete(childEntry, timestamp, visited);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates audit logs for all tracked entity changes.
    /// </summary>
    /// <param name="entityEntries">All tracked entities</param>
    /// <param name="userId">ID of user making the changes</param>
    /// <param name="timestamp">Timestamp of update</param>
    /// <returns>List of audit logs to be added to the database</returns>
    private static List<AuditLog> CreateAuditLogs(IEnumerable<EntityEntry> entityEntries, string? userId, DateTime timestamp)
    {
        var auditLogs = new List<AuditLog>();
        var userIdValue = userId ?? string.Empty;

        foreach (var entry in entityEntries)
        {
            // Skip audit logs themselves and unchanged/detached entities
            if (entry.Entity is AuditLog ||
                entry.State == EntityState.Detached ||
                entry.State == EntityState.Unchanged)
            {
                continue;
            }

            var actionType = GetActionType(entry.State);
            if (actionType == null)
            {
                continue;
            }

            var log = new AuditLog
            {
                UserId = userIdValue,
                ActionType = actionType.Value.ToString(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.State == EntityState.Added
                    ? "N/A"
                    : entry.CurrentValues["Id"]?.ToString() ?? "N/A",
                Timestamp = timestamp,
                Changes = actionType.Value switch
                {
                    ActionType.Created => SerializeValues(entry.CurrentValues.Properties, entry.CurrentValues),
                    ActionType.Updated => SerializeDifferences(entry),
                    ActionType.Deleted => SerializeValues(entry.OriginalValues.Properties, entry.OriginalValues),
                    _ => "{}"
                }
            };

            auditLogs.Add(log);
        }

        return auditLogs;
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