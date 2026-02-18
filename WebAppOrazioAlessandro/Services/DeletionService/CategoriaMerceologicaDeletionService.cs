using WebAppOrazioAlessandro.Data;

namespace WebAppOrazioAlessandro.Services.DeletionService
{
    public class CategoriaMerceologicaDeletionService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaMerceologicaDeletionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorie.FindAsync(id);
            if (categoria == null) return;

            _context.Categorie.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}
