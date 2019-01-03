using Microsoft.EntityFrameworkCore;

// Context for querying the DB

namespace TheWall.Models
{
    public class WallContext : DbContext
    {
        // Link to the DB
        public WallContext(DbContextOptions<WallContext> options) : base(options) {}

         // <---------- Table names ---------->

        // For Accounts Table
        public DbSet<Account> Accounts { get; set; }

        // For Messages Table
        public DbSet<Message> Messages { get; set; }

        // For Comments Table
        public DbSet<Comment> Comments { get; set; }
    }
}
