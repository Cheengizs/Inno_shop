using System.Collections;
using Users.Application.DTOs;
using Users.Application.Results;

namespace Users.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserServiceResult<IEnumerable<UserResponse>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10);
    Task<UserServiceResult<UserResponse>> RegisterAsync(RegisterRequest request);
    Task<UserServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<UserServiceResult<UserResponse>> GetByIdAsync(int id);
    Task<UserServiceResult> SendConfirmationEmailAsync(int userId);
    Task<UserServiceResult> ConfirmEmailAsync(string token); 
    Task<UserServiceResult> ChangeActiveStatusAsync(int userId, bool isActive);
    Task<UserServiceResult<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<UserServiceResult> ForgotPasswordAsync(string email);
    Task<UserServiceResult> ResetPasswordAsync(ResetPasswordRequest request);
}