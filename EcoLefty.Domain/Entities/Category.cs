using EcoLefty.Domain.Common;

namespace EcoLefty.Domain.Entities;

public class Category : SoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; }
    public virtual ICollection<ApplicationUser> FollowingUsers { get; set; }
}
