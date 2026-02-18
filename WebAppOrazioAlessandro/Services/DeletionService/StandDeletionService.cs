using WebAppOrazioAlessandro.Data;

namespace WebAppOrazioAlessandro.Services.DeletionService
{
    public class StandDeletionService
    {
        private readonly ApplicationDbContext _context;

        public StandDeletionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(int id)
        {
            var stand = await _context.Stands.FindAsync(id);
            if (stand == null) return;

            _context.Stands.Remove(stand);
            await _context.SaveChangesAsync();
        }
    }
}
