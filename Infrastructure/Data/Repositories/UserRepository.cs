using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.Entities;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(AppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }
        public async Task RegisterAsync(string name, string email, string password, UserRole role)
        {
            var passwordHash = _passwordHasher.HashPassword(password);
            var user = new User(name, email, passwordHash, role);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveConfirmationTokenAsync(Guid userId, string token, DateTime expiresAt)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ArgumentException($"Пользователь не найден");

            user.ConfirmationToken = token;
            user.ConfirmationTokenExpiresAt = expiresAt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByConfirmationTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ConfirmationToken == token);
        }

        public async Task SavePasswordResetTokenAsync(Guid userId, string token, DateTime expiresAt)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ArgumentException($"Пользователь не найден");

            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpireAt = expiresAt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == token);
        }

        public async Task UpdateUserPasswordAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
