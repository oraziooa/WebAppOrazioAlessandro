using WebAppOrazioAlessandro.Data;

namespace WebAppOrazioAlessandro.Services.DeletionService
{
    public class SettoreDeletionService
    {
        private readonly ApplicationDbContext _context;

        public SettoreDeletionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(int id)
        {
            var settore = await _context.Settori.FindAsync(id);
            if (settore == null) return;

            _context.Settori.Remove(settore);
            await _context.SaveChangesAsync();
        }
    }
}
