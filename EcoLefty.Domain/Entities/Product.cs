using EcoLefty.Domain.Common;

namespace EcoLefty.Domain.Entities;

public class Product : SoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public string ImageUrl { get; set; }

    // Foreign keys
    public int CompanyId { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; }
    public virtual ICollection<Category> Categories { get; set; }
    public virtual ICollection<Offer> Offers { get; set; }
}
