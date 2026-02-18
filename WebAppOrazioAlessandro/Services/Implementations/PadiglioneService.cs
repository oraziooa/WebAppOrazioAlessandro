using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAppOrazioAlessandro.Data;
using WebAppOrazioAlessandro.DTOs.CategoriaMerceologica;
using WebAppOrazioAlessandro.DTOs.Padiglione;
using WebAppOrazioAlessandro.Entities;
using WebAppOrazioAlessandro.Hubs;
using WebAppOrazioAlessandro.Services.Interfaces;

public class PadiglioneService : IPadiglioneService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public PadiglioneService(ApplicationDbContext context,
                             IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task<IEnumerable<PadiglioneResponseDto>> GetAllAsync()
    {
        return await _context.Padiglioni
            .Select(p => new PadiglioneResponseDto
            {
                Id = p.Id,
                Nome = p.Nome
            })
            .ToListAsync();
    }

    public async Task<PadiglioneResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Padiglioni
            .Where(p => p.Id == id)
            .Select(p => new PadiglioneResponseDto
            {
                Id = p.Id,
                Nome = p.Nome
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PadiglioneResponseDto> CreateAsync(CreatePadiglioneDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Il nome del padiglione non può essere vuoto.");

        bool exists = await _context.Padiglioni
            .AnyAsync(p => p.Nome.ToLower() == dto.Nome.Trim().ToLower());
        if (exists)
            throw new InvalidOperationException("Esiste già un padiglione con questo nome.");

        var entity = new Padiglione
        {
            Nome = dto.Nome.Trim()
        };

        _context.Padiglioni.Add(entity);
        await _context.SaveChangesAsync();

        var response = new PadiglioneResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };

        await _hub.Clients.All.SendAsync(
            "EntityCreated",
            nameof(Padiglione),
            response);

        return response;
    }

    public async Task<PadiglioneResponseDto?> UpdateAsync(int id, CreatePadiglioneDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Il nome del padiglione non può essere vuoto.");

        var entity = await _context.Padiglioni.FindAsync(id);
        if (entity == null)
            return null;

        // Controllo i duplicati 
        bool exists = await _context.Padiglioni
            .AnyAsync(p => p.Id != id && p.Nome.ToLower() == dto.Nome.Trim().ToLower());
        if (exists)
            throw new InvalidOperationException("Esiste già un padiglione con questo nome.");

        entity.Nome = dto.Nome.Trim();
        await _context.SaveChangesAsync();

        var response = new PadiglioneResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };

        await _hub.Clients.All.SendAsync(
            "EntityUpdated",
            nameof(Padiglione),
            response);

        return response;
    }


    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Padiglioni.FindAsync(id);
        if (entity == null)
            return false;

        var response = new PadiglioneResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };

        _context.Padiglioni.Remove(entity);
        await _context.SaveChangesAsync();

        await _hub.Clients.All.SendAsync(
            "EntityDeleted",
            nameof(Padiglione),
            response);

        return true;
    }

}
