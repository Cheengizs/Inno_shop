using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.DbConfiguring;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure.DbContexts;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }
        
    public DbSet<UserEntity> Users { get; set; } = null!;
}