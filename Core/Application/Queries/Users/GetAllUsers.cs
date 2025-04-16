using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;


namespace Shop.Core.Application.Queries.Users
{
    public class GetAllUsersQuery : IRequest<IEnumerable<User>> { }

    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetAllUsersAsync();
        }
    }
}

