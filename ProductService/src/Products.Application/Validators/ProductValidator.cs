using System.Data;
using FluentValidation;
using Products.Application.DTOs.Products;
using Products.Domain.Constants;

namespace Products.Application.Validators;

public class ProductValidator : AbstractValidator<ProductRequest>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(ProductConstants.ProductNameMaxLength).WithMessage($"Name must not exceed {ProductConstants.ProductNameMaxLength} characters");
        
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required")
            .GreaterThan(ProductConstants.ProductPriceMinValue).WithMessage($"Price must be greater than {ProductConstants.ProductPriceMinValue}");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(ProductConstants.ProductDescriptionMaxLength).WithMessage($"Description must not exceed {ProductConstants.ProductDescriptionMaxLength} characters");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be greater than 0");

        RuleFor(x => x.IsAvailable)
            .NotEmpty().WithMessage("IsAvailable is required");
    }
}