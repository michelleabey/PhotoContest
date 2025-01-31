using Microsoft.EntityFrameworkCore;
using PhotoContest.Models;

namespace PhotoContest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Prevent duplicate votes
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.UserId, v.PhotoId })
                .IsUnique();
        }
    }
}
