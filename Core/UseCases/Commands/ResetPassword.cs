using System;
using System.Threading.Tasks;
using Shop.Core.Interfaces;

namespace Shop.Core.UseCases.Commands
{
    public class ResetPasswordCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public ResetPasswordCommand(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task ExecuteAsync(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token), "Токен не может быть пустым");
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword), "Пароль не может быть пустым");

            var user = await _userRepository.GetUserByResetTokenAsync(token);

            if (user == null || user.ResetPasswordTokenExpireAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Токен не действителен");
            }

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);

            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpireAt = null;

            await _userRepository.UpdateUserAsync(user);
        }
    }
}
