using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shop.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActivated { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public string? ConfirmationToken { get; set; }
        public DateTime? ConfirmationTokenExpiresAt { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpireAt { get; set; }

        public User(string name, string email, string passwordHash, UserRole role)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            IsActivated = true;
            CreatedAt = DateTime.UtcNow;

            ConfirmationToken = null;
            ConfirmationTokenExpiresAt = null;

            ResetPasswordToken = null;
            ResetPasswordTokenExpireAt = null;
        }

        public User() { }

        public bool Activate() => IsActivated = true;
        public bool Deactivate() => IsActivated = false;
    }

    public enum UserRole
    {
        Customer,
        Seller,
        Admin
    }
}