using Microsoft.EntityFrameworkCore;

namespace CachingMVC.Models
{
    public class MobileContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public MobileContext(DbContextOptions<MobileContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}