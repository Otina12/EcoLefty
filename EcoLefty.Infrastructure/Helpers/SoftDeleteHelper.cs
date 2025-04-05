using EcoLefty.Domain.Common;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

internal static class SoftDeleteHelper
{
    public static void ProcessChanges(EcoLeftyDbContext context, ICurrentUserContext userContext, DateTime timestamp)
    {
        var entityEntries = context.ChangeTracker.Entries().ToList();
        var softDeletedEntities = new List<EntityEntry>();
        var logs = new List<AuditLog>();

        foreach (var entry in entityEntries)
        {
            var log = AuditLogWriter.CreateAuditLog(entry, userContext.UserId, timestamp);
            if (log != null)
            {
                logs.Add(log);
            }
        }

        ApplySoftDeleteAndTimestamps(entityEntries, timestamp, softDeletedEntities);

        foreach (var entry in softDeletedEntities)
        {
            var log = AuditLogWriter.CreateAuditLog(entry, userContext.UserId, timestamp, forceTreatAsDeleted: true);
            if (log != null)
            {
                logs.Add(log);
            }
        }

        context.AddRange(logs);
    }

    private static void ApplySoftDeleteAndTimestamps(IEnumerable<EntityEntry> entityEntries, DateTime timestamp, List<EntityEntry> softDeletedEntities)
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
                    CascadeSoftDelete(entry, timestamp, new HashSet<object>(), softDeletedEntities);
                    break;
            }
        }
    }

    private static void CascadeSoftDelete(EntityEntry parentEntry, DateTime timestamp, HashSet<object> visited, List<EntityEntry> softDeletedEntities)
    {
        if (visited.Contains(parentEntry.Entity))
            return;

        visited.Add(parentEntry.Entity);

        foreach (var navigation in parentEntry.Navigations)
        {
            if (!navigation.IsLoaded)
                navigation.Load();

            if (navigation.Metadata is ISkipNavigation)
                continue;

            var navigationMetadata = navigation.Metadata;

            if (navigationMetadata is not INavigation navMetadata)
                continue;

            if (navMetadata.IsOnDependent)
                continue;

            if (navigationMetadata.IsCollection && navigation.CurrentValue is IEnumerable<object> children)
            {
                foreach (var child in children)
                {
                    HandleSoftDeleteChild(parentEntry, timestamp, visited, softDeletedEntities, child);
                }
            }
            else if (navigation.CurrentValue is object child)
            {
                HandleSoftDeleteChild(parentEntry, timestamp, visited, softDeletedEntities, child);
            }
        }
    }

    private static void HandleSoftDeleteChild(EntityEntry parentEntry, DateTime timestamp, HashSet<object> visited, List<EntityEntry> softDeletedEntities, object child)
    {
        if (child is SoftDeletableEntity softChild && softChild.DeletedAtUtc == null)
        {
            softChild.DeletedAtUtc = timestamp;
            var childEntry = parentEntry.Context.Entry(child);

            if (childEntry.State != EntityState.Deleted)
            {
                childEntry.State = EntityState.Modified;
            }

            softDeletedEntities.Add(childEntry);
            CascadeSoftDelete(childEntry, timestamp, visited, softDeletedEntities);
        }
    }
}
