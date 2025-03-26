using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Application.Authentication;

public interface IAuthenticationService
{
    Task<string> GenerateJwtToken(Account account);
}
