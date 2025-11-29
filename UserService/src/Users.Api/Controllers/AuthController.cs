using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Commons;
using Users.Application.DTOs;
using Users.Application.Services.Interfaces;

namespace Users.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                ServiceErrorCode.Validation => BadRequest(result.Errors),
                ServiceErrorCode.Conflict => Conflict(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => Unauthorized(new { error = "Invalid credentials" }), 
                ServiceErrorCode.Unauthorized => Unauthorized(result.Errors),
                ServiceErrorCode.Forbidden => StatusCode(403, result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }
        
        return Ok(result.Value);
    }

    [HttpPost("send-confirmation-email")]
    [Authorize] 
    public async Task<IActionResult> SendConfirmationEmailAsync()
    {
        var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var result = await _userService.SendConfirmationEmailAsync(userId);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(result.Errors),
                ServiceErrorCode.Conflict => Conflict(result.Errors), 
                _ => StatusCode(500, result.Errors)
            };
        }

        return Ok(new { message = "Verification email sent." });
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token is required.");
        }

        var result = await _userService.ConfirmEmailAsync(token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { message = "Email confirmed successfully!" });
    }
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var result = await _userService.RefreshTokenAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                ServiceErrorCode.Validation => BadRequest(result.Errors),
                ServiceErrorCode.Unauthorized => Unauthorized(result.Errors), // 401 - пусть фронт выкидывает на логин
                ServiceErrorCode.Forbidden => StatusCode(403, result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return Ok(result.Value);
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
    {
        var result = await _userService.ForgotPasswordAsync(request.Email);

        if (!result.IsSuccess)
        {
            return StatusCode(500, result.Errors);
        }

        return Ok(new { message = "If an account with that email exists, a reset link has been sent." });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var result = await _userService.ResetPasswordAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                ServiceErrorCode.Validation => BadRequest(result.Errors), 
                ServiceErrorCode.NotFound => BadRequest(result.Errors),   
                _ => StatusCode(500, result.Errors)
            };
        }

        return Ok(new { message = "Password has been reset successfully." });
    }
}