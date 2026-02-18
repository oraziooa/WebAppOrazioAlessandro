using WebAppOrazioAlessandro.Data;

namespace WebAppOrazioAlessandro.Services.DeletionService
{
    public class PadiglioneDeletionService
    {
        private readonly ApplicationDbContext _context;

        public PadiglioneDeletionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(int id)
        {
            var padiglione = await _context.Padiglioni.FindAsync(id);
            if (padiglione == null) return;

            _context.Padiglioni.Remove(padiglione);
            await _context.SaveChangesAsync();
        }
    }
}
