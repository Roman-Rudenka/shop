using System;
using System.Threading.Tasks;
using Shop.Core.Interfaces;

namespace Shop.Core.Services
{
    public class PasswordResetService
    {
        private readonly IUserRepository _userRepository;

        public PasswordResetService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task GeneratePasswordResetTokenAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Пользователь не найден");
            }

            user.ResetPasswordToken = Guid.NewGuid().ToString();
            user.ResetPasswordTokenExpireAt = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateUserAsync(user);
        }
    }
}
