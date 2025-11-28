namespace Products.Application.DTOs.Products;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    int UserId,
    DateTime CreatedAt);