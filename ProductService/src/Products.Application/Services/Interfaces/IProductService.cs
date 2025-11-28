using Products.Application.DTOs.Products;
using Products.Application.Filters;
using Products.Application.Results;

namespace Products.Application.Services.Interfaces;

public interface IProductService
{
    Task<ProductServiceResult<ProductResponse>> GetByIdAsync(int id);
    Task<ProductServiceResult<IEnumerable<ProductResponse>>> GetAllAsync(ProductFilter? filter = null);
    Task<ProductServiceResult<IEnumerable<ProductResponse>>> GetPagedAsync(int pageNumber, int pageSize, ProductFilter? filter = null);
    Task<ProductServiceResult<ProductResponse>> AddAsync(ProductRequest request);
    Task<ProductServiceResult<ProductResponse>> UpdateAsync(int id, ProductRequest request);
    Task<ProductServiceResult> DeleteAsync(int id, int userId);
    Task<ProductServiceResult<IEnumerable<ProductResponse>>> GetByOwnerIdAsync(int ownerId);
}