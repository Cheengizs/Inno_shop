using Users.Application.DTOs;
using Users.Application.Results;
using Users.Domain.Models;

namespace Users.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserServiceResult<UserResponse>> RegisterAsync(RegisterRequest request);
    Task<UserServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<UserServiceResult> ValidateUserEmail(int userId);
}