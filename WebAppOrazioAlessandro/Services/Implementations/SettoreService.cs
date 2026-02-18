using Microsoft.EntityFrameworkCore;
using WebAppOrazioAlessandro.Entities;
using WebAppOrazioAlessandro.DTOs.Settore;
using WebAppOrazioAlessandro.Services.Interfaces;
using WebAppOrazioAlessandro.Data;
using WebAppOrazioAlessandro.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace WebAppOrazioAlessandro.Services.Implementations
{
    public class SettoreService : ISettoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public SettoreService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<IEnumerable<SettoreResponseDto>> GetAllAsync()
        {
            return await _context.Settori
                .Include(s => s.Padiglione)
                .Select(s => new SettoreResponseDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    PadiglioneId = s.PadiglioneId,
                    PadiglioneNome = s.Padiglione != null ? s.Padiglione.Nome : string.Empty
                })
                .ToListAsync();
        }

        public async Task<SettoreResponseDto?> GetByIdAsync(int id)
        {
            var settore = await _context.Settori
                .Include(s => s.Padiglione)
                .FirstOrDefaultAsync(s => s.Id == id);

            return settore == null ? null : MapToResponse(settore);
        }

        public async Task<SettoreResponseDto> CreateAsync(CreateSettoreDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Il nome del settore non può essere vuoto.");

            // Controllo duplicati nello stesso padiglione
            bool exists = await _context.Settori
                .AnyAsync(s => s.PadiglioneId == dto.PadiglioneId
                               && s.Nome.ToLower() == dto.Nome.Trim().ToLower());
            if (exists)
                throw new InvalidOperationException("Esiste già un settore con questo nome nello stesso padiglione.");

            var settore = new Settore
            {
                Nome = dto.Nome.Trim(),
                PadiglioneId = dto.PadiglioneId
            };

            _context.Settori.Add(settore);
            await _context.SaveChangesAsync();

            var response = await GetByIdAsync(settore.Id)
                           ?? throw new Exception("Errore creazione Settore");

            await _hub.Clients.All.SendAsync(
                "EntityCreated",
                nameof(Settore),
                response);

            return response;
        }

        public async Task<SettoreResponseDto?> UpdateAsync(int id, CreateSettoreDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Il nome del settore non può essere vuoto.");

            var settore = await _context.Settori.FindAsync(id);
            if (settore == null) return null;

            // Controllo duplicati nello stesso padiglione escludendo l'entità corrente
            bool exists = await _context.Settori
                .AnyAsync(s => s.Id != id
                               && s.PadiglioneId == dto.PadiglioneId
                               && s.Nome.ToLower() == dto.Nome.Trim().ToLower());
            if (exists)
                throw new InvalidOperationException("Esiste già un settore con questo nome nello stesso padiglione.");

            settore.Nome = dto.Nome.Trim();
            settore.PadiglioneId = dto.PadiglioneId;

            await _context.SaveChangesAsync();

            var response = await GetByIdAsync(id);
            if (response == null) return null;

            await _hub.Clients.All.SendAsync(
                "EntityUpdated",
                nameof(Settore),
                response);

            return response;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var settore = await _context.Settori
                .Include(s => s.Padiglione)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (settore == null) return false;

            var response = MapToResponse(settore);

            _context.Settori.Remove(settore);
            await _context.SaveChangesAsync();

            await _hub.Clients.All.SendAsync(
                "EntityDeleted",
                nameof(Settore),
                response);

            return true;
        }

        // Metodo privato per evitare duplicazione mapping
        private static SettoreResponseDto MapToResponse(Settore s)
        {
            return new SettoreResponseDto
            {
                Id = s.Id,
                Nome = s.Nome,
                PadiglioneId = s.PadiglioneId,
                PadiglioneNome = s.Padiglione?.Nome ?? string.Empty
            };
        }
    }
}
