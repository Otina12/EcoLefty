namespace EcoLefty.Domain.Entities;

/// <summary>
/// Represents an audit log entry that records changes made to entities in the system.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Gets or sets the unique ID for the audit log entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who made the change.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the type of action performed (Create, Update, Delete...).
    /// </summary>
    public string ActionType { get; set; }

    /// <summary>
    /// Gets or sets the name of the entity that was changed.
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the affected entity.
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// Gets or sets the JSON-formatted string detailing the changes made.
    /// </summary>
    public string Changes { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the change was recorded.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Returns a formatted string that represents the audit log entry.
    /// </summary>
    /// <returns>A formatted string containing the audit log details.</returns>
    public override string ToString()
    {
        return $"Id: {Id}\n" +
               $"UserId: {UserId}\n" +
               $"ActionType: {ActionType}\n" +
               $"Entity: {EntityName} (Id: {EntityId})\n" +
               $"Changes: {Changes}\n" +
               $"Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}";
    }
}
