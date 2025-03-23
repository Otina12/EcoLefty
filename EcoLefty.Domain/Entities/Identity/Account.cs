using EcoLefty.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace EcoLefty.Domain.Entities.Identity;

public class Account : IdentityUser<string>
{
    public bool IsActive { get; set; }
    public AccountType AccountType { get; set; }
}