using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Extensions;
using Products.Application.Commons;
using Products.Application.DTOs.Products;
using Products.Application.Filters;
using Products.Application.Results;
using Products.Application.Services.Interfaces;

namespace Products.Api.Controllers;

[ApiController]
[Route("/api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetProductByIdAsync(int id)
    {
        ProductServiceResult<ProductResponse> serviceResult = await _productService.GetByIdAsync(id);
        if (!serviceResult.IsSuccess)
        {
            if (serviceResult.ErrorCode == ServiceErrorCode.NotFound)
                return NotFound();

            return BadRequest(serviceResult.Errors);
        }

        ProductResponse product = serviceResult.Value;
        return Ok(product);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ProductResponse>>> GetAllProductsAsync([FromQuery] ProductFilter? filter)
    {
        var serviceResult = await _productService.GetAllAsync(filter);

        if (!serviceResult.IsSuccess)
            return BadRequest(serviceResult.Errors);

        return Ok(serviceResult.Value);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetPagedAsync([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10, [FromQuery] ProductFilter? filter = null)
    {
        var serviceResult = await _productService.GetPagedAsync(pageNumber, pageSize, filter);

        if (!serviceResult.IsSuccess)
            return BadRequest(serviceResult.Errors);

        var result = serviceResult.Value;
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductCreateRequest request)
    {
        var userId = User.GetUserId();

        if (userId == null)
            return Unauthorized();

        ProductRequest productToAdd = new ProductRequest(
            Name: request.Name,
            Description: request.Description,
            Price: request.Price,
            IsAvailable: request.IsAvailable,
            UserId: userId.Value
        );

        var result = await _productService.AddAsync(productToAdd);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == ServiceErrorCode.Validation)
                return BadRequest();

            return BadRequest(result.Errors);
        }

        ProductResponse product = result.Value;

        return CreatedAtAction(nameof(GetProductByIdAsync), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductCreateRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null) return Unauthorized();

        var productToUpdate = new ProductRequest(
            Name: request.Name,
            Description: request.Description,
            Price: request.Price,
            IsAvailable: request.IsAvailable,
            UserId: userId.Value
        );

        var result = await _productService.UpdateAsync(id, productToUpdate);

        if (!result.IsSuccess)
            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(result.Errors),
                ServiceErrorCode.Forbidden => StatusCode(403, result.Errors),
                _ => BadRequest(result.Errors)
            };
        
        var product = result.Value;

        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductAsync(int id)
    {
        var userId = User.GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _productService.DeleteAsync(id, userId.Value);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
    
    [HttpPatch("internal/user-status/{userId:int}")]
    public async Task<IActionResult> UpdateUserStatusAsync(int userId, [FromBody] bool isActive)
    {
        await _productService.UpdateProductsStatusByUserAsync(userId, isActive);
        return NoContent();
    }
}