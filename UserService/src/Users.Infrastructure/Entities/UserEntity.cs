using Users.Domain.Enums;

namespace Users.Infrastructure.Entities;

public class UserEntity
{
    public int Id { get; set; } = 0;
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}