using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Alumni> Alumni { get; set; }
    public DbSet<Mention> Mentions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связей
        modelBuilder.Entity<Mention>()
            .HasOne(m => m.Alumni)
            .WithMany(a => a.Mentions)
            .HasForeignKey(m => m.AlumniId);
    }
}