using EcoLefty.Domain.Common;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Domain.Entities;

public class ApplicationUser : SoftDeletableEntity
{
    // Primary key and foreign key to Account at the same time
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public DateTime BirthDate { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public decimal Balance { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
    public virtual ICollection<Category> FollowedCategories { get; set; }
    public virtual ICollection<Purchase> Purchases { get; set; }
}
