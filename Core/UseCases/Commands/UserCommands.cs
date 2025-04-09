using Shop.Core.Entities;
using Shop.Core.Interfaces;
using Shop.API.Configuration;

namespace Shop.Core.UseCases.Commands
{
    public class UserCommands
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;


        public UserCommands(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        public async Task RegisterUserAsync(string name, string email, string password, UserRole role)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Имя пользователя не может быть пустым.");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email не может быть пустым.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Пароль не может быть пустым.");

            }

            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException("Пользователь с таким email уже существует.");
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(password),
                Role = role,
                IsActivated = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);
        }


        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.AuthenticateAsync(email, password);
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



        public async Task UpdateUserAsync(Guid id, string? name, string? email, string? password, UserRole role)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                throw new ArgumentException($"Пользователь с id {id} не найден.");
            }

            if (!string.IsNullOrEmpty(name))
            {
                user.Name = name;
            }

            if (!string.IsNullOrEmpty(email))
            {
                user.Email = email;
            }

            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(password);
            }

            user.Role = role;

            await _userRepository.UpdateUserAsync(user);
        }


        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            await _userRepository.DeleteUserAsync(user);
        }

        public async Task ActivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            user.Activate();
            await _userRepository.UpdateUserAsync(user);

        }

        public async Task DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");

            }

            user.Deactivate();
            await _userRepository.UpdateUserAsync(user);
        }
    }
}
