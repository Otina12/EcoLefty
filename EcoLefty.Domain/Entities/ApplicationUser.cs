using EcoLefty.Domain.Common;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Domain.Entities;

public class ApplicationUser : SoftDeletableEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string ProfilePictureUrl { get; set; }
    public decimal Balance { get; set; }

    // Foreign keys
    public string AuthUserId { get; set; }

    // Navigation properties
    public virtual UserAccount IdentityUser { get; set; }
    public virtual ICollection<Category> FollowedCategories { get; set; }
    public virtual ICollection<Company> Companies { get; set; }
}
