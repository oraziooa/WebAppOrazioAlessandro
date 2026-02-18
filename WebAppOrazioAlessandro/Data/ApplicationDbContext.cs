using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppOrazioAlessandro.Entities;

namespace WebAppOrazioAlessandro.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Padiglione> Padiglioni => Set<Padiglione>();
        public DbSet<Settore> Settori => Set<Settore>();
        public DbSet<CategoriaMerceologica> Categorie => Set<CategoriaMerceologica>();
        public DbSet<Stand> Stands => Set<Stand>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Settore>()
                .HasOne(s => s.Padiglione)
                .WithMany(p => p.Settori)
                .HasForeignKey(s => s.PadiglioneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Stand>()
                .HasOne(s => s.Settore)
                .WithMany(s => s.Stands)
                .HasForeignKey(s => s.SettoreId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
