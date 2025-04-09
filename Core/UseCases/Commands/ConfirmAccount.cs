using System;
using System.Threading.Tasks;
using Shop.Core.Interfaces;

namespace Shop.Core.UseCases.Commands
{
    public class ConfirmAccountCommand
    {
        private readonly IUserRepository _userRepository;

        public ConfirmAccountCommand(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(string token)
        {
            var user = await _userRepository.GetUserByConfirmationTokenAsync(token);

            if (user == null || user.ConfirmationTokenExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Токен истек.");
            }

            user.IsActivated = true;
            user.ConfirmationToken = null;
            user.ConfirmationTokenExpiresAt = null;

            await _userRepository.UpdateUserAsync(user);
        }
    }
}
