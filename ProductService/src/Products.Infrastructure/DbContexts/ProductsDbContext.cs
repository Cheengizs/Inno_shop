using Microsoft.EntityFrameworkCore;
using Products.Infrastructure.DbConfigurations;
using Products.Infrastructure.Entities;

namespace Products.Infrastructure.DbContexts;

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }
    
    public DbSet<ProductEntity> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
    }
}