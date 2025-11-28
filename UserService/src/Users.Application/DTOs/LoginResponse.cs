namespace Users.Application.DTOs;

public record LoginResponse(
    string Token,
    string RefreshToken,
    int UserId,
    string Username,
    string Email,
    string Role
);