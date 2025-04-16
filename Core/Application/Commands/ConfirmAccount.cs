using MediatR;
using Shop.Core.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Application.Commands
{
    public class ConfirmAccountCommand : IRequest<Unit>
    {
        public string Token { get; set; }

        public ConfirmAccountCommand(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }

    public class ConfirmAccountHandler : IRequestHandler<ConfirmAccountCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public ConfirmAccountHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByConfirmationTokenAsync(request.Token);

            if (user == null || user.ConfirmationTokenExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Токен истек.");
            }

            user.IsActivated = true;
            user.ConfirmationToken = null;
            user.ConfirmationTokenExpiresAt = null;

            await _userRepository.UpdateUserAsync(user);

            return Unit.Value;
        }
    }
}
