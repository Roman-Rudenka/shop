using System.Text;
using System.Security.Cryptography;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Пароль не может быть пустым");
            }
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}