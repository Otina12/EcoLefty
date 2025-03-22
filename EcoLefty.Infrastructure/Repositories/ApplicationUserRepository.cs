using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class ApplicationUserRepository : BaseRepository<ApplicationUser, int>, IApplicationUserRepository
{
    public ApplicationUserRepository(EcoLeftyDbContext context) : base(context)
    {
    }
}