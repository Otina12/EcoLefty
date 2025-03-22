using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class OfferRepository : BaseRepository<Offer, int>, IOfferRepository
{
    public OfferRepository(EcoLeftyDbContext context) : base(context)
    {
    }
}