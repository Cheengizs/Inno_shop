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
    private readonly IEmailTokenService _emailTokenService;

    public AuthController(IUserService userService,
        IEmailTokenService emailTokenService)
    {
        _userService = userService;
        _emailTokenService = emailTokenService;
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
            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ServiceErrorCode.NotFound => NotFound(result.Errors),
                    ServiceErrorCode.Unauthorized => Unauthorized(result.Errors),
                    _ => StatusCode(500, result.Errors)
                };
            }
        }
        
        return Ok(result.Value);
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string token)
    {
        if (!_emailTokenService.ValidateEmailConfirmationToken(token, out var userId))
        {
            return BadRequest("Invalid or expired confirmation token.");
        }

        await _userService.ValidateUserEmail(userId);
        return Ok("Email confirmed successfully!");
    }
    
}