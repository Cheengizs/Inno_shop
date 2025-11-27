namespace Products.Application.Dto_s.Products;

public class ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    int UserId,
    DateTime CreatedAt);