using FluentValidation;
using Products.Api.Common.DTOs;

public class CreateProductValidator : AbstractValidator<CreateProductDto> {
  public CreateProductValidator() {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    RuleFor(x => x.Price).GreaterThan(0);
    RuleFor(x => x.Sku).NotEmpty();
  }
}
