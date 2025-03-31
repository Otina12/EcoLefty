using EcoLefty.Domain.Common;
using EcoLefty.Domain.Common.Enums;

namespace EcoLefty.Domain.Entities;

public class Offer : SoftDeletableEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; } // unit price (discounted) of a product
    public int TotalQuantity { get; set; } // quantity of a product
    public int QuantityAvailable { get; set; } // quantity of products left
    public OfferStatus OfferStatus { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }

    // Foreign keys
    public int ProductId { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual ICollection<Purchase> Purchases { get; set; }

}
