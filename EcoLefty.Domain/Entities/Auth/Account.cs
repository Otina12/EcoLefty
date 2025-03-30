using EcoLefty.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace EcoLefty.Domain.Entities.Identity;

public class Account : IdentityUser
{
    public bool IsActive { get; set; }
    public AccountRole AccountType { get; set; }
}