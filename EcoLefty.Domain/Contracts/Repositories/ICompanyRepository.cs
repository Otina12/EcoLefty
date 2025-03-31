using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Domain.Contracts.Repositories;

public interface ICompanyRepository : IBaseRepository<Company, string>
{
}
