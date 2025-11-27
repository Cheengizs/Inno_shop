using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Domain.Constants;
using Products.Infrastructure.Entities;

namespace Products.Infrastructure.DbConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> entity)
    {
        entity
            .HasKey(p => p.Id);
        
        entity
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(p => p.Name)
            .HasMaxLength(ProductConstants.ProductNameMaxLength)
            .IsRequired();
        
        entity
            .Property(p => p.Description)
            .HasMaxLength(ProductConstants.ProductDescriptionMaxLength)
            .IsRequired();
        
        entity.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        entity.Property(x => x.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        entity.Property(x => x.UserId)
            .IsRequired();
    }
}