namespace Products.Application.DTOs.Products;

public record ProductCreateRequest(
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    DateTime CreatedAt);