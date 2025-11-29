using Products.Application.Filters;
using Products.Domain.Models;

namespace Products.Application.RepositoryInterfaces;

public interface IProductRepository
{
    Task<ProductModel?> GetByIdAsync(int id);
    Task<IEnumerable<ProductModel>> GetAllAsync(ProductFilter? filter = null);

    Task<IEnumerable<ProductModel>> GetPagedAsync(int pageNumber,
        int pageSize,
        ProductFilter? filter = null
        );

    Task<ProductModel> AddAsync(ProductModel product);
    Task UpdateAsync(ProductModel product);
    Task DeleteAsync(int id);
    Task UpdateDeletionStatusAsync(int id, bool isDeleted);
    Task<IEnumerable<ProductModel>> GetByOwnerIdAsync(int ownerId);
    Task UpdateUserActiveStatusAsync(int userId, bool isUserActive);
}