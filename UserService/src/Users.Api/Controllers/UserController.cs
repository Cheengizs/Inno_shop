using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Users.Application.Commons;
using Users.Application.DTOs;
using Users.Application.Services.Interfaces;

namespace Users.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}/status")]
    public async Task<ActionResult<UserStatusDto>> GetUserStatus(int id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.IsSuccess || result.Value is null) return NotFound();

        var statusDto = new UserStatusDto
        {
            UserId = result.Value.Id,
            EmailConfirmed = result.Value.EmailConfirmed,
            IsActive = result.Value.IsActive
        };

        return Ok(statusDto);
    }

    [HttpPatch("{id:int}/activation")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleUserActivationAsync(int id, [FromBody] bool isActive)
    {
        var result = await _userService.ChangeActiveStatusAsync(id, isActive);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return Ok(new { userId = id, isActive = isActive, message = "User status updated." });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsersAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetAllUsersAsync(pageNumber, pageSize);

        if (!result.IsSuccess)
        {
            return StatusCode(500, result.Errors);
        }

        return Ok(result.Value);
    }
}