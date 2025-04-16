using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Commands.Users
{
    public class UpdateUserCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public string? Name { get; }
        public string? Email { get; }
        public string? Password { get; }
        public UserRole Role { get; }

        public UpdateUserCommand(Guid id, string? name, string? email, string? password, UserRole role)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Password))
                user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            user.Role = request.Role;

            await _userRepository.UpdateUserAsync(user);
            return Unit.Value;
        }
    }
}
