using Microsoft.AspNetCore.Mvc;

namespace Users.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Ok();
    }
    
}