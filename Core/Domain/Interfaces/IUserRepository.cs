using Shop.Core.Domain.Entities;

namespace Shop.Core.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> AuthenticateAsync(string email, string password);
        Task RegisterAsync(string name, string email, string password, UserRole role);

        Task SaveConfirmationTokenAsync(Guid userId, string token, DateTime expiresAt);
        Task<User?> GetUserByConfirmationTokenAsync(string token);
        Task SavePasswordResetTokenAsync(Guid userId, string token, DateTime expiresAt);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task UpdateUserPasswordAsync(User user);
    }
}
