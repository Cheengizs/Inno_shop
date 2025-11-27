using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Constants;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure.DbConfiguring;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> entity)
    {
        entity
            .HasKey(x => x.Id);
        
        entity
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(x => x.Username)
            .HasMaxLength(UserConstants.UserUsernameMaxLength)
            .IsRequired();
        
        entity
            .HasIndex(x => x.Username)
            .IsUnique();

        entity
            .Property(x => x.FirstName)
            .HasMaxLength(UserConstants.UserFirstNameMaxLength)
            .IsRequired();

        entity
            .Property(x => x.LastName)
            .HasMaxLength(UserConstants.UserLastNameMaxLength)
            .IsRequired();

        entity
            .Property(x => x.Email)
            .HasMaxLength(UserConstants.UserEmailMaxLength)
            .IsRequired();

        entity
            .HasIndex(x => x.Email)
            .IsUnique();

        entity
            .Property(x => x.PasswordHash)
            .IsRequired();

        entity
            .Property(x => x.Role)
            .HasConversion<string>() 
            .IsRequired();

        entity
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        entity
            .Property(x => x.EmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        entity
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}