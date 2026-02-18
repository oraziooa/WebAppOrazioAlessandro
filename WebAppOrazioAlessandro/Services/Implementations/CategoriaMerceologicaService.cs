using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAppOrazioAlessandro.Data;
using WebAppOrazioAlessandro.DTOs.CategoriaMerceologica;
using WebAppOrazioAlessandro.Entities;
using WebAppOrazioAlessandro.Hubs;
using WebAppOrazioAlessandro.Services.Interfaces;

public class CategoriaMerceologicaService : ICategoriaMerceologicaService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public CategoriaMerceologicaService(
        ApplicationDbContext context,
        IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task<IEnumerable<CategoriaResponseDto>> GetAllAsync()
    {
        return await _context.Categorie
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                Nome = c.Nome
            })
            .ToListAsync();
    }

    public async Task<CategoriaResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Categorie
            .Where(c => c.Id == id)
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                Nome = c.Nome
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoriaResponseDto> CreateAsync(CreateCategoriaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Il nome della categoria non può essere vuoto.");

        // Controllo duplicati
        bool exists = await _context.Categorie
            .AnyAsync(c => c.Nome.ToLower() == dto.Nome.Trim().ToLower());
        if (exists)
            throw new InvalidOperationException("Esiste già una categoria con questo nome.");

        var entity = new CategoriaMerceologica
        {
            Nome = dto.Nome.Trim()
        };

        _context.Categorie.Add(entity);
        await _context.SaveChangesAsync();

        var response = new CategoriaResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };

        await _hub.Clients.All.SendAsync(
            "EntityCreated",
            nameof(CategoriaMerceologica),
            response);

        return response;
    }

    public async Task<CategoriaResponseDto?> UpdateAsync(int id, CreateCategoriaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Il nome della categoria non può essere vuoto.");

        var entity = await _context.Categorie.FindAsync(id);
        if (entity == null)
            return null;

        // Controllo duplicati escludendo l'entità corrente
        bool exists = await _context.Categorie
            .AnyAsync(c => c.Id != id && c.Nome.ToLower() == dto.Nome.Trim().ToLower());
        if (exists)
            throw new InvalidOperationException("Esiste già una categoria con questo nome.");

        entity.Nome = dto.Nome.Trim();
        await _context.SaveChangesAsync();

        var response = new CategoriaResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };

        await _hub.Clients.All.SendAsync(
            "EntityUpdated",
            nameof(CategoriaMerceologica),
            response);

        return response;
    }


    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Categorie.FindAsync(id);
        if (entity == null)
            return false;

        var response = new CategoriaResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };

        _context.Categorie.Remove(entity);
        await _context.SaveChangesAsync();

        await _hub.Clients.All.SendAsync(
            "EntityDeleted",
            nameof(CategoriaMerceologica),
            response);

        return true;
    }
}
