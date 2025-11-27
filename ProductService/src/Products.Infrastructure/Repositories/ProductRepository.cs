using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Products.Application.Filters;
using Products.Application.RepositoryInterfaces;
using Products.Domain.Models;
using Products.Infrastructure.DbContexts;
using Products.Infrastructure.Entities;

namespace Products.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;
    private readonly IMapper _mapper;

    public ProductRepository(ProductsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductModel?> GetByIdAsync(int id)
    {
        var productFromDb = await _context.Products.FindAsync(id);
        var result = _mapper.Map<ProductModel>(productFromDb);
        return result;
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync(ProductFilter? filter = null)
    {
        var query = _context.Products.AsQueryable();
        if (filter != null)
        {
            if (filter.UserId.HasValue)
                query = query.Where(p => p.UserId == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NameContains))
                query = query.Where(p => p.Name.Contains(filter.NameContains));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.IsAvailable.HasValue)
                query = query.Where(p => p.IsAvailable == filter.IsAvailable.Value);
        }

        var products = await query.ToListAsync();
        var result = _mapper.Map<IEnumerable<ProductModel>>(products);
        return result;
    }

    public async Task<IEnumerable<ProductModel>> GetPagedAsync(int pageNumber, int pageSize, ProductFilter? filter = null)
    {
        var query = _context.Products.AsQueryable();

        if (filter != null)
        {
            if (filter.UserId.HasValue)
                query = query.Where(p => p.UserId == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NameContains))
                query = query.Where(p => p.Name.Contains(filter.NameContains));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.IsAvailable.HasValue)
                query = query.Where(p => p.IsAvailable == filter.IsAvailable.Value);
        }
        
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);
        
        query = query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking();
        
        var products = await query.ToListAsync();
        var result = _mapper.Map<IEnumerable<ProductModel>>(products);
        return result;
    }

    public async Task<ProductModel> AddAsync(ProductModel product)
    {
        var productToAdd = _mapper.Map<ProductEntity>(product);
        await _context.Products.AddAsync(productToAdd);
        await _context.SaveChangesAsync();
        ProductModel result = _mapper.Map<ProductModel>(productToAdd);
        return result;
    }


    public async Task UpdateAsync(ProductModel product)
    {
        var productFromDb = await _context.Products.FindAsync(product.Id);
        if (productFromDb == null) return;
        if (productFromDb.UserId != product.UserId) return; 

        _mapper.Map(product, productFromDb);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductModel>> GetByOwnerIdAsync(int ownerId)
    {
        var products = await _context.Products
            .Where(p => p.UserId == ownerId)
            .ToListAsync();

        var result = _mapper.Map<IEnumerable<ProductModel>>(products);
        return result;
    }
}