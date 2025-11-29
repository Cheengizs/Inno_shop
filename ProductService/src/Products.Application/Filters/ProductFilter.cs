namespace Products.Application.Filters;

public class ProductFilter
{
    public int? UserId { get; set; } = null;
    public string? NameContains { get; set; } = null;
    public decimal? MinPrice { get; set; } = null;
    public decimal? MaxPrice { get; set; } = null;
    public bool? IsAvailable { get; set; } = null;
    public bool? RemoveDeleted { get; set; } = null;
    public bool? RemoveNonActiveUsers { get; set; } = null;
}