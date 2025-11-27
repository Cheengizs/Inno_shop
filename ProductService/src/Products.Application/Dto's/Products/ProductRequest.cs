namespace Products.Application.Dto_s.Products;

public record ProductRequest(
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    int UserId);