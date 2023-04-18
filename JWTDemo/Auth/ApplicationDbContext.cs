using JWTDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTDemo.Auth
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<UserTbl> Users { get; set; }
        public DbSet<Product> products { get; set; }
    }
}
