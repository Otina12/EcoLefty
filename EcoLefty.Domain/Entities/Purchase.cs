using EcoLefty.Domain.Common;
using EcoLefty.Domain.Common.Enums;

namespace EcoLefty.Domain.Entities;

public class Purchase : SoftDeletableEntity
{
    public int Id { get; set; }
    public PurchaseStatus PurchaseStatus { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime PurchaseDateUtc { get; set; }

    // Foreign keys
    public int OfferId { get; set; }
    public string CustomerId { get; set; }

    // Navigation properties
    public virtual Offer Offer { get; set; }
    public virtual ApplicationUser Customer { get; set; }
}
