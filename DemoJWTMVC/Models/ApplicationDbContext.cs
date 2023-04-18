using Microsoft.EntityFrameworkCore;

namespace DemoJWTMVC.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<UserTbl> Users { get; set; }
        public DbSet<Product> products { get; set; }
    }
}
