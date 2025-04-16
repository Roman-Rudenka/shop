using MediatR;
using Shop.Core.Domain.Interfaces;
using Shop.API.Configuration;
using Shop.Core.Domain.Configuration;

namespace Shop.Core.Application.Commands.Users
{
    public class LoginCommand : IRequest<string>
    {
        public string Email { get; }
        public string Password { get; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Неверный email или пароль.");
            }

            return JwtConfiguration.GenerateToken(
                _configuration,
                user.Id,
                user.Name ?? throw new ArgumentNullException(nameof(user.Name)),
                user.Email ?? throw new ArgumentNullException(nameof(user.Email)),
                user.Role.ToString()
            );
        }
    }
}
