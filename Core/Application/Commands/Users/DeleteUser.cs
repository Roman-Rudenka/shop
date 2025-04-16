using MediatR;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Commands.Users
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; }

        public DeleteUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            await _userRepository.DeleteUserAsync(user);
            return Unit.Value;
        }
    }
}
