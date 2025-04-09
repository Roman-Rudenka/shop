using FluentValidation;
using Shop.Core.Interfaces;

namespace Shop.Core.Entities.Validators
{
    public class FluentUserValidator : AbstractValidator<User>
    {
        public FluentUserValidator(IUserRepository userRepository)
        {
            RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Имя обязательно")
            .Length(2, 20).WithMessage("Имя должно быть от 2 до 20 симловолов");

            RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неверный формат почты")
            .MustAsync(async (email, cancellation)=> !await userRepository.EmailExistsAsync(email)).WithMessage("Почта уже используется");

            RuleFor(user => user.PasswordHash)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен быть не менее 6 символов");
        }
    }
}