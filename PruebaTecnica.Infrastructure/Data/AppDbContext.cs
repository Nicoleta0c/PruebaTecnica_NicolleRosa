using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PruebaTecnica.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Configuración del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Name);

            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasDefaultValue(0);
        }
    }
}
