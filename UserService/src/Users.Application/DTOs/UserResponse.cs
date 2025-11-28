using Users.Domain.Enums;

namespace Users.Application.DTOs;

public record UserResponse(
    int Id,
    string Username,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    bool IsActive,
    bool EmailConfirmed,
    DateTime CreatedAt);