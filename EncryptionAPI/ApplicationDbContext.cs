using EncryptionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EncryptionAPI;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserKey> UserKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<User>()
            .HasOne(u => u.Key)
            .WithOne(k => k.User)
            .HasForeignKey<UserKey>(k => k.UserId);
    }
}
