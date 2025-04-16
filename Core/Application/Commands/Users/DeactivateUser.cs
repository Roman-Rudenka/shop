using MediatR;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Commands.Users
{
    public class DeactivateUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; }

        public DeactivateUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public DeactivateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            user.Deactivate();
            await _userRepository.UpdateUserAsync(user);

            return Unit.Value;
        }
    }
}
