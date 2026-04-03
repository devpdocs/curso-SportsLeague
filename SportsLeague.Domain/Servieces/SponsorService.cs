using Microsoft.Extensions.Logging;

using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Interfaces.Repositories;

using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService

{

    private readonly ISponsorRepository _sponsorRepository;

    private readonly ITournamentRepository _tournamentRepository;

    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;

    private readonly ILogger<SponsorService> _logger;

    public SponsorService(

            ISponsorRepository sponsorRepository,

            ITournamentRepository tournamentRepository,

            ITournamentSponsorRepository tournamentSponsorRepository,

            ILogger<SponsorService> logger)

    {

        _sponsorRepository = sponsorRepository;

        _tournamentRepository = tournamentRepository;

        _tournamentSponsorRepository = tournamentSponsorRepository;

        _logger = logger;

    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()

    {

        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();

    }

    public async Task<Sponsor?> GetByIdAsync(int id)

    {

        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);

        var sponsor = await _sponsorRepository.GetByIdAsync(id);

        if (sponsor == null)

            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

        return sponsor;

    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)

    {

        // Validación de negocio: nombre único

        var existingSponsor = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);

        if (existingSponsor != null)

        {

            _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);

            throw new InvalidOperationException(

            $"Ya existe un Sponsor con el nombre '{sponsor.Name}'");

        }

        _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);

        return await _sponsorRepository.CreateAsync(sponsor);

    }

    public async Task UpdateAsync(int id, Sponsor sponsor)

    {

        var existingSponsor = await _sponsorRepository.GetByIdAsync(id);

        if (existingSponsor == null)

        {

            _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);

            throw new KeyNotFoundException(

            $"No se encontró el sponsor con ID {id}");

        }

        // Validar nombre único (si cambió)

        if (existingSponsor.Name != sponsor.Name)

        {

            var sponsorWithSameName = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);

            if (sponsorWithSameName != null)

            {

                throw new InvalidOperationException(

                $"Ya existe un sponsor con el nombre '{sponsor.Name}'");

            }

        }

        existingSponsor.Name = sponsor.Name;

        existingSponsor.ContactEmail = sponsor.ContactEmail;

        existingSponsor.Phone = sponsor.Phone;

        existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;

        existingSponsor.Category = sponsor.Category;

        _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);

        await _sponsorRepository.UpdateAsync(existingSponsor);

    }

    public async Task DeleteAsync(int id)

    {

        var exists = await _sponsorRepository.ExistsAsync(id);

        if (!exists)

        {

            _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);

            throw new KeyNotFoundException(

            $"No se encontró el sponsor con ID {id}");

        }

        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);

        await _sponsorRepository.DeleteAsync(id);

    }

    public async Task AddTournamentSponsorAsync(int sponsorId, int tournamentId, decimal contractAmount)
    {

        var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

        if (sponsor == null) throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");


        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null) throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");


        var existing = await _tournamentSponsorRepository

            .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

        if (existing != null) throw new InvalidOperationException("Este sponsor ya está vinculado al torneo");


        if (contractAmount <= 0) throw new InvalidOperationException("El monto del contrato debe ser mayor a 0");

        var entity = new TournamentSponsor
        {
            SponsorId = sponsorId,

            TournamentId = tournamentId,

            ContractAmount = contractAmount,

            JoinedAt = DateTime.UtcNow
        };

        _logger.LogInformation(

            "Linking sponsor {SponsorId} to tournament {TournamentId}",

            sponsorId, tournamentId);

        await _tournamentSponsorRepository.CreateAsync(entity);
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

        if (sponsor == null)
            throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");

        var relations = await _tournamentSponsorRepository
            .GetBySponsorAsync(sponsorId);

        return relations.Select(ts => ts.Tournament);
    }

    public async Task RemoveTournamentSponsorAsync(int sponsorId, int tournamentId)
    {
        var existing = await _tournamentSponsorRepository
            .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

        if (existing == null) throw new KeyNotFoundException("La relación no existe");

        await _tournamentSponsorRepository.DeleteAsync(existing.Id);
    }

}
