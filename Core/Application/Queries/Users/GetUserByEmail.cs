using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Queries.Users
{
    public class GetUserByEmailQuery : IRequest<User?>
    {
        public string Email { get; }

        public GetUserByEmailQuery(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }

    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, User?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByEmailAsync(request.Email);
        }
    }
}
