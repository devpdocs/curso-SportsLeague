using Microsoft.EntityFrameworkCore;

using SportsLeague.DataAccess.Context;

using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository

{

    public SponsorRepository(LeagueDbContext context) : base(context)

    {

    }

    public async Task<Sponsor?> ExistsByNameAsync(string sponsorName)
    {
        return await _dbSet
        .FirstOrDefaultAsync(t => t.Name.ToLower() == sponsorName.ToLower());

    }

}
