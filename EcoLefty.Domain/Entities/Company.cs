using EcoLefty.Domain.Common;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Domain.Entities;

public class Company : SoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string? LogoUrl { get; set; }
    public decimal Balance { get; set; }
    public bool IsApproved { get; set; }

    // Foreign keys
    public string AccountId { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
    public virtual ICollection<Product> Products { get; set; }
}
