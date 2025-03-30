using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Domain.Entities.Auth;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresOnUtc { get; set; }

    // Foreign keys
    public string AccountId { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
}
