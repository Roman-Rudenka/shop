using MediatR;
using Shop.Core.Domain.Interfaces;


namespace Shop.Core.Application.Commands
{
    public class ResetPasswordCommand : IRequest<Unit>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }

        public ResetPasswordCommand(string token, string newPassword)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            NewPassword = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
        }
    }

    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByResetTokenAsync(request.Token);

            if (user == null || user.ResetPasswordTokenExpireAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Токен не действителен.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpireAt = null;

            await _userRepository.UpdateUserAsync(user);

            return Unit.Value;
        }
    }
}
