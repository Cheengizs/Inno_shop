namespace Products.Application.DTOs.Products;

public record ProductRequest(
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    int UserId);