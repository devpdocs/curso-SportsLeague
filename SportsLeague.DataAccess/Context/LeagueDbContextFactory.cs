using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SportsLeague.DataAccess.Context
{
    public class LeagueDbContextFactory : IDesignTimeDbContextFactory<LeagueDbContext>
    {
        public LeagueDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LeagueDbContext>();
            optionsBuilder.UseSqlite("Data Source=./Data/LocalDB.db");
            // or: optionsBuilder.UseNpgsql(...) / UseSqlite(...) etc.

            return new LeagueDbContext(optionsBuilder.Options);
        }
    }
}
