using MediatR;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Commands.Users
{
    public class ActivateUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; }

        public ActivateUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public ActivateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            user.Activate();
            await _userRepository.UpdateUserAsync(user);

            return Unit.Value;
        }
    }
}
