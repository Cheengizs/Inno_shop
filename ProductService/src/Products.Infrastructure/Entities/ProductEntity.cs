namespace Products.Infrastructure.Entities;

public class ProductEntity
{
    public int Id { get; init; } = 0;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; } = 0;
    public bool IsAvailable { get; set; } = true;
    public int UserId { get; set; } = 0; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}