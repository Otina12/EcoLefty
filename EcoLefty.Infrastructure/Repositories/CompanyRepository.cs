using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class CompanyRepository : BaseRepository<Company, string>, ICompanyRepository
{
    public CompanyRepository(EcoLeftyDbContext context) : base(context)
    {
    }
}