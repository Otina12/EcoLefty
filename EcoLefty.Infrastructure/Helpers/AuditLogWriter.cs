using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

internal static class AuditLogWriter
{
    public static AuditLog? CreateAuditLog(EntityEntry entry, string? userId, DateTime timestamp, bool forceTreatAsDeleted = false)
    {
        if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            return null;

        var actionType = forceTreatAsDeleted
            ? ActionType.Deleted
            : GetActionType(entry.State);

        if (actionType == null)
        {
            return null;
        }

        var id = entry.CurrentValues["Id"]?.ToString() ?? "N/A";

        var log = new AuditLog
        {
            UserId = userId ?? string.Empty,
            ActionType = actionType.ToString()!,
            EntityName = entry.Entity.GetType().Name,
            EntityId = id,
            Timestamp = timestamp,
            Changes = actionType switch
            {
                ActionType.Created => SerializeValues(entry.CurrentValues.Properties, entry.CurrentValues),
                ActionType.Updated => SerializeDifferences(entry),
                ActionType.Deleted => SerializeValues(entry.OriginalValues.Properties, entry.OriginalValues),
                _ => "{}"
            }
        };

        return log;
    }

    public static List<AuditLog> CreateAuditLogs(IEnumerable<EntityEntry> entries, string? userId, DateTime timestamp, bool forceTreatAsDeleted = false)
    {
        return entries
            .Select(entry => CreateAuditLog(entry, userId, timestamp, forceTreatAsDeleted))
            .Where(log => log != null)
            .Cast<AuditLog>()
            .ToList();
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
        var dict = properties.ToDictionary(p => p.Name, p => values[p.Name]);
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
