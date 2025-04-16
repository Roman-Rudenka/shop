using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Commands.Users
{
    public class RegisterUserCommand : IRequest<Unit>
    {
        public string Name { get; }
        public string Email { get; }
        public string Password { get; }
        public UserRole Role { get; }

        public RegisterUserCommand(string name, string email, string password, UserRole role)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Пользователь с таким email уже существует.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = request.Role,
                IsActivated = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);
            return Unit.Value;
        }
    }
}
