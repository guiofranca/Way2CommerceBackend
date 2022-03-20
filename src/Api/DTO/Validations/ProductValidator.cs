using Api.DTO.Product;
using Domain.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.DTO.Validations;
public class ProductValidator : AbstractValidator<ProductRequest>
{
    protected readonly ICategoryRepository _categoryRepository;
    protected readonly IProductRepository _productRepository;
    private int? _id = null;

    public ProductValidator(ICategoryRepository categoryRepository, IProductRepository productRepository, IHttpContextAccessor httpContext)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;

        int tempId;
        _id = Int32.TryParse((string?)httpContext.HttpContext?.Request.RouteValues["id"], out tempId) ? tempId : null;

        RuleFor(x => x.Code).Cascade(CascadeMode.Stop)
            .Length(1, 6)
            .MustAsync(BeUniqueCode)
            .WithMessage("This code is already used");

        RuleFor(x => x.Name).Length(3, 255);
        RuleFor(x => x.Description).Length(4, 255).When(x => x.Description != String.Empty);
        RuleFor(x => x.Price).NotEmpty().NotNull().ScalePrecision(2, 18);
        RuleFor(x => x.CategoryIds).Cascade(CascadeMode.Stop)
            .NotNull()
            .ForEach(x => x.GreaterThan(0))
            .DependentRules(() => RuleFor(x => x.CategoryIds)
                .MustAsync(BeExistingCategories)
                .WithMessage("Category not found"));
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken arg2)
    {
        return await _productRepository.GetByCodeAsync(code, _id) == null;
    }

    private async Task<bool> BeExistingCategories(IEnumerable<int> ids, CancellationToken arg2)
    {
        var categories = await _categoryRepository.GetByIdAsync(ids);
        return categories.Count() == ids.Count();
    }
}
