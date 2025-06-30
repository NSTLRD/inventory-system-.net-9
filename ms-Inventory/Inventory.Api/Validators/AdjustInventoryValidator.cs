using FluentValidation;
using Inventory.Api.Application.Commands;

namespace Inventory.Api.Validators
{
    public class AdjustInventoryValidator : AbstractValidator<AdjustInventory>
    {
        public AdjustInventoryValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");
        }
    }
}
