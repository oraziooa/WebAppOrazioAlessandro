using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAppOrazioAlessandro.Data;
using WebAppOrazioAlessandro.DTOs.Stand;
using WebAppOrazioAlessandro.Entities;
using WebAppOrazioAlessandro.Services.Interfaces;
using WebAppOrazioAlessandro.Hubs;

namespace WebAppOrazioAlessandro.Services.Implementations
{
    public class StandService : IStandService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public StandService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<IEnumerable<StandResponseDto>> GetAllAsync()
        {
            return await _context.Stands
                .Include(s => s.Settore)
                .Include(s => s.CategoriaMerceologica)
                .Select(s => new StandResponseDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    SettoreId = s.SettoreId,
                    SettoreNome = s.Settore != null ? s.Settore.Nome : string.Empty,
                    CategoriaMerceologicaId = s.CategoriaMerceologicaId,
                    CategoriaNome = s.CategoriaMerceologica != null ? s.CategoriaMerceologica.Nome : string.Empty
                })
                .ToListAsync();
        }


        public async Task<StandResponseDto?> GetByIdAsync(int id)
        {
            var stand = await _context.Stands
                .Include(s => s.Settore)
                .Include(s => s.CategoriaMerceologica)
                .FirstOrDefaultAsync(s => s.Id == id);

            return stand == null ? null : MapToResponse(stand);
        }

        public async Task<StandResponseDto> CreateAsync(CreateStandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Il nome dello stand non può essere vuoto.");

            // Controllo duplicati nello stesso Settore e Categoria
            bool exists = await _context.Stands
                .AnyAsync(s => s.SettoreId == dto.SettoreId
                               && s.CategoriaMerceologicaId == dto.CategoriaMerceologicaId
                               && s.Nome.ToLower() == dto.Nome.Trim().ToLower());
            if (exists)
                throw new InvalidOperationException("Esiste già uno stand con questo nome nello stesso settore e categoria.");

            var stand = new Stand
            {
                Nome = dto.Nome.Trim(),
                SettoreId = dto.SettoreId,
                CategoriaMerceologicaId = dto.CategoriaMerceologicaId
            };

            _context.Stands.Add(stand);
            await _context.SaveChangesAsync();

            var response = await GetByIdAsync(stand.Id)
                           ?? throw new Exception("Errore creazione Stand");

            await _hub.Clients.All.SendAsync(
                "EntityCreated",
                nameof(Stand),
                response);

            return response;
        }

        public async Task<StandResponseDto?> UpdateAsync(int id, CreateStandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Il nome dello stand non può essere vuoto.");

            var stand = await _context.Stands.FindAsync(id);
            if (stand == null) return null;

            // Controllo duplicati nello stesso Settore e Categoria escludendo l'entità corrente
            bool exists = await _context.Stands
                .AnyAsync(s => s.Id != id
                               && s.SettoreId == dto.SettoreId
                               && s.CategoriaMerceologicaId == dto.CategoriaMerceologicaId
                               && s.Nome.ToLower() == dto.Nome.Trim().ToLower());
            if (exists)
                throw new InvalidOperationException("Esiste già uno stand con questo nome nello stesso settore e categoria.");

            stand.Nome = dto.Nome.Trim();
            stand.SettoreId = dto.SettoreId;
            stand.CategoriaMerceologicaId = dto.CategoriaMerceologicaId;

            await _context.SaveChangesAsync();

            var response = await GetByIdAsync(id);
            if (response == null) return null;

            await _hub.Clients.All.SendAsync(
                "EntityUpdated",
                nameof(Stand),
                response);

            return response;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var stand = await _context.Stands
                .Include(s => s.Settore)
                .Include(s => s.CategoriaMerceologica)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stand == null) return false;

            var response = MapToResponse(stand);

            _context.Stands.Remove(stand);
            await _context.SaveChangesAsync();

            await _hub.Clients.All.SendAsync(
                "EntityDeleted",
                nameof(Stand),
                response);

            return true;
        }

        private static StandResponseDto MapToResponse(Stand s)
        {
            return new StandResponseDto
            {
                Id = s.Id,
                Nome = s.Nome,
                SettoreId = s.SettoreId,
                SettoreNome = s.Settore?.Nome ?? string.Empty,
                CategoriaMerceologicaId = s.CategoriaMerceologicaId,
                CategoriaNome = s.CategoriaMerceologica?.Nome ?? string.Empty
            };
        }
    }
}
