using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Domain.Contracts.Repositories;

public interface IApplicationUserRepository : IBaseRepository<ApplicationUser, int>
{
}
