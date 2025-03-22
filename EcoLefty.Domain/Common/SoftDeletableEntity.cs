namespace EcoLefty.Domain.Common;

public class SoftDeletableEntity
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}