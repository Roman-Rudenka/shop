using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Shop.API.Configuration;
using Shop.Core.Entities;
using Shop.Core.Interfaces;
using Shop.Core.UseCases.Commands;
using Shop.Core.UseCases.Queries;
using Shop.Core.Entities.Validators;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;
using Shop.Core.Services;

namespace Shop.API
{
    public class Startup
    {
        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IValidator<User>, FluentUserValidator>();
            services.AddScoped<IValidator<Product>, ProductValidator>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IEmailRepository, EmailService>();

            services.AddScoped<ConfirmAccountCommand>();
            services.AddScoped<ResetPasswordCommand>();
            services.AddScoped<PasswordResetService>();


            services.AddJwtAuthentication(_configuration);

            services.AddScoped<UserCommands>();
            services.AddScoped<UserQueries>();

            services.AddScoped<ProductCommands>();
            services.AddScoped<ProductQueries>();

            services.AddAuthorization();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseRouting();


            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                DatabaseInit.InitializeAsync(services).Wait();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
