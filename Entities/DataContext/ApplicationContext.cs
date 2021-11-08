using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }
        public DbSet<UrlStatistics> UrlStatistics { get; set; }
    }
}