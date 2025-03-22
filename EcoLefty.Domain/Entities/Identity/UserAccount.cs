using Microsoft.AspNetCore.Identity;

namespace EcoLefty.Domain.Entities.Identity;

public class UserAccount : IdentityUser<string>
{
    public bool IsActive { get; set; }
}
