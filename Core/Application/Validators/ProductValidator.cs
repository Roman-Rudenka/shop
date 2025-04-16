using FluentValidation;
using Shop.Core.Domain.Entities;

namespace Shop.Core.Application.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Название продукта обязательно.")
                .MaximumLength(100).WithMessage("Название не должно превышать 100 символов.");

            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов.");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0.");

            RuleFor(p => p.PublisherId)
                .NotEmpty().WithMessage("Идентификатор продавца обязателен.");
        }
    }
}
