using Match.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Match.Data
{
    public class MatchDbContext : IdentityDbContext<User>
    {
        public MatchDbContext(DbContextOptions<MatchDbContext> options) : base(options)
        {

        }
        public new DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
