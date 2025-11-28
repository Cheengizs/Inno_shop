using AutoMapper;
using FluentValidation;
using Products.Application.Clients;
using Products.Application.Commons;
using Products.Application.DTOs.Products;
using Products.Application.Filters;
using Products.Application.RepositoryInterfaces;
using Products.Application.Results;
using Products.Application.Services.Interfaces;
using Products.Domain.Models;

namespace Products.Application.Services;

public class ProductService : IProductService
{

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ProductRequest> _productRequestValidator;
    private readonly IUserServiceClient _userServiceClient;
    
    public ProductService(IProductRepository productRepository, IMapper mapper, IValidator<ProductRequest> productRequestValidator, IUserServiceClient userServiceClient)
    {
        _productRequestValidator = productRequestValidator;
        _productRepository = productRepository;
        _mapper = mapper;
        _userServiceClient = userServiceClient;
    }
    
    public async Task<ProductServiceResult<ProductResponse>> GetByIdAsync(int id)
    {
        var productFromRepo = await _productRepository.GetByIdAsync(id);

        if (productFromRepo == null)
        {
            ProductServiceResult<ProductResponse> failResult = ProductServiceResult<ProductResponse>.Failure(["Product not found."], ServiceErrorCode.NotFound);
            return failResult;
        }
        
        ProductResponse productResult = _mapper.Map<ProductResponse>(productFromRepo);
        ProductServiceResult<ProductResponse> successResult =
            ProductServiceResult<ProductResponse>.Success(productResult);
        
        return successResult;
    }

    public async Task<ProductServiceResult<IEnumerable<ProductResponse>>> GetAllAsync(ProductFilter? filter = null)
    {
        var productsFromRepo = await _productRepository.GetAllAsync(filter);
        var productsResult = _mapper.Map<IEnumerable<ProductResponse>>(productsFromRepo);
        var result = ProductServiceResult<IEnumerable<ProductResponse>>.Success(productsResult);
            
        return result;
    }

    public async Task<ProductServiceResult<IEnumerable<ProductResponse>>> GetPagedAsync(int pageNumber, int pageSize, ProductFilter? filter = null)
    {
        var productsFromRepo = await _productRepository.GetPagedAsync(pageNumber, pageSize, filter);
        var productsResult = _mapper.Map<IEnumerable<ProductResponse>>(productsFromRepo);
        var result = ProductServiceResult<IEnumerable<ProductResponse>>.Success(productsResult);
        return result;
    }

    public async Task<ProductServiceResult<ProductResponse>> AddAsync(ProductRequest request)
    {
        var validationResult = await _productRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ProductServiceResult<ProductResponse>.Failure(
                validationResult.Errors.Select(e => e.ErrorMessage).ToList(), ServiceErrorCode.Validation);
        }

        bool isEmailConfirmed = await _userServiceClient.IsEmailConfirmedAsync(request.UserId);
    
        if (!isEmailConfirmed)
        {
            return ProductServiceResult<ProductResponse>.Failure(
                ["Email is not confirmed. Please verify your email to create products."], 
                ServiceErrorCode.Forbidden);
        }

        var productEntity = _mapper.Map<ProductModel>(request);
        var responseFromRepo = await _productRepository.AddAsync(productEntity);
        ProductResponse productResult = _mapper.Map<ProductResponse>(responseFromRepo);
    
        return ProductServiceResult<ProductResponse>.Success(productResult);
    }
    
    public async Task<ProductServiceResult<ProductResponse>> UpdateAsync(int id, ProductRequest request)
    {
        var validationResult = await _productRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var failResult = ProductServiceResult<ProductResponse>.Failure(
                validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                ServiceErrorCode.Validation);
            return failResult;
        }

        var isEmailConfirmed = await _userServiceClient.IsEmailConfirmedAsync(request.UserId);
        if (!isEmailConfirmed)
        {
            return ProductServiceResult<ProductResponse>.Failure(
                ["Email is not confirmed. Cannot update products."], ServiceErrorCode.Forbidden);
        }
        
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            return ProductServiceResult<ProductResponse>.Failure(
                ["Product not found."],
                ServiceErrorCode.NotFound);
        }

        if (existingProduct.UserId != request.UserId)
        {
            var failResult =
                ProductServiceResult<ProductResponse>.Failure(["forbidden user"], ServiceErrorCode.Forbidden);
            return failResult;
        }
        
        
        _mapper.Map(request, existingProduct); 
        await _productRepository.UpdateAsync(existingProduct);

        
        
        ProductResponse productResult = _mapper.Map<ProductResponse>(existingProduct);
        var result = ProductServiceResult<ProductResponse>.Success(productResult);
        
        return result;
    }

    public async Task<ProductServiceResult> DeleteAsync(int id, int userId)
    {
        var productFromRepo = await _productRepository.GetByIdAsync(id);
        if (productFromRepo == null)
        {
            var failResult = ProductServiceResult.Failure(["Product not found."], ServiceErrorCode.NotFound);
            return failResult;
        }

        if (productFromRepo.UserId != userId)
        {
            var failResult = ProductServiceResult.Failure(["forbidden user"], ServiceErrorCode.Forbidden);
            return failResult;
        }
        
        var isEmailConfirmed = await _userServiceClient.IsEmailConfirmedAsync(userId);
        if (!isEmailConfirmed)
        {
            return ProductServiceResult.Failure(
                ["Email is not confirmed. Cannot update products."], ServiceErrorCode.Forbidden);
        }

        await _productRepository.DeleteAsync(id);
        var result = ProductServiceResult.Success();
        return result;
    }

    public async Task<ProductServiceResult<IEnumerable<ProductResponse>>> GetByOwnerIdAsync(int ownerId)
    {
        if (!(await _userServiceClient.ExistsAsync(ownerId)))
        {
            var failResult = ProductServiceResult<IEnumerable<ProductResponse>>.Failure(["User does not exist."], ServiceErrorCode.NotFound);
            return failResult;
        }
        
        var productsFromRepo = await _productRepository.GetByOwnerIdAsync(ownerId);
        var productsResult = _mapper.Map<IEnumerable<ProductResponse>>(productsFromRepo);
        var result = ProductServiceResult<IEnumerable<ProductResponse>>.Success(productsResult);
        return result;
    }
}