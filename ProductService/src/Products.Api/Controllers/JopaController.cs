using Microsoft.AspNetCore.Mvc;
using Products.Application.Services.Interfaces;

namespace Products.Api.Controllers;

[ApiController]
[Route("api/products")]
public class JopaController : ControllerBase
{
    private readonly IProductService _productService;

    public JopaController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var x = await _productService.GetByOwnerIdAsync(1);
        if (!x.IsSuccess)
        {
            return BadRequest();
        }
        
        return Ok();
    }
}