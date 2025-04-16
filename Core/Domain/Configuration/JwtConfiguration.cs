using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Shop.Core.Domain.Configuration
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var keyString = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key не указан в конфигурации");
            var key = Encoding.UTF8.GetBytes(keyString);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer не указан в конфигурации"),
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience не указан в конфигурации"),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static string GenerateToken(IConfiguration configuration, Guid userId, string name, string email, string role)
        {
            var keyString = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key не указан в конфигурации");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer не указан в конфигурации");
            var audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience не указан в конфигурации");

            if (!double.TryParse(configuration["Jwt:ExpiryMinutes"], out var expiryMinutes))
            {
                throw new FormatException("Jwt:ExpiryMinutes имеет неверный формат");
            }

            var claims = new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("name", name),
                new Claim("email", email),
                new Claim("role", role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
