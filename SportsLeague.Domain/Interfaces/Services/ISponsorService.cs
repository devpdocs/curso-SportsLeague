using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services;

public interface ISponsorService

{

    Task<IEnumerable<Sponsor>> GetAllAsync();

    Task<Sponsor?> GetByIdAsync(int id);

    Task<Sponsor> CreateAsync(Sponsor referee);

    Task UpdateAsync(int id, Sponsor referee);

    Task DeleteAsync(int id);

    Task AddTournamentSponsorAsync(int sponsorId, int tournamentId, decimal contractAmount);

    Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId);

    Task RemoveTournamentSponsorAsync(int sponsorId, int tournamentId);

}
