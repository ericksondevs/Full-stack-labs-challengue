using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configures one-to-many relationship
            modelBuilder.Entity<Url>()
                .HasOne<UrlStatistics>(s => s.UrlStatistics)
                .WithMany(g => g.Urls).HasForeignKey(s => s.UrlStatId);
        }

    public DbSet<Url> Urls { get; set; }
        public DbSet<UrlStatistics> UrlStatistics { get; set; }
    }
}