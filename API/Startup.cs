using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Shop.API.Configuration;
using Shop.Core.Application.Commands.Products;
using Shop.Core.Application.Commands.Users;
using Shop.Core.Application.Queries.Products;
using Shop.Core.Application.Queries.Users;
using Shop.Core.Application.Commands;
using Shop.Core.Application.Queries;
using Shop.Core.Application.Validators;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;
using Shop.Core.Domain.Configuration;


namespace Shop.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

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
            services.AddScoped<PasswordResetService>();

            services.AddMediatR(typeof(RegisterUserCommand).Assembly);
            services.AddMediatR(typeof(LoginCommand).Assembly);
            services.AddMediatR(typeof(UpdateUserCommand).Assembly);
            services.AddMediatR(typeof(DeleteUserCommand).Assembly);
            services.AddMediatR(typeof(ActivateUserCommand).Assembly);
            services.AddMediatR(typeof(DeactivateUserCommand).Assembly);
            services.AddMediatR(typeof(GetAllUsersQuery).Assembly);
            services.AddMediatR(typeof(GetUserByIdQuery).Assembly);
            services.AddMediatR(typeof(GetUserByEmailQuery).Assembly);

            services.AddMediatR(typeof(AddProductCommand).Assembly);
            services.AddMediatR(typeof(DeleteProductCommand).Assembly);
            services.AddMediatR(typeof(HideProductsByPublisherCommand).Assembly);
            services.AddMediatR(typeof(ShowProductsByPublisherCommand).Assembly);
            services.AddMediatR(typeof(UpdateProductCommand).Assembly);
            services.AddMediatR(typeof(FilterByPriceQuery).Assembly);
            services.AddMediatR(typeof(FilterBySellerQuery).Assembly);
            services.AddMediatR(typeof(GetAllProductsQuery).Assembly);
            services.AddMediatR(typeof(GetProductByIdQuery).Assembly);
            services.AddMediatR(typeof(GetProductsByPublisherQuery).Assembly);
            services.AddMediatR(typeof(SearchProductsByNameQuery).Assembly);

            services.AddMediatR(typeof(ConfirmAccountCommand).Assembly);
            services.AddMediatR(typeof(ResetPasswordCommand).Assembly);

            services.AddJwtAuthentication(_configuration);


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

            services.AddAuthorization();

            services.AddSwaggerConfiguration();
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
            app.UseSwagger();
            app.UseSwaggerUI();

        }
    }
}
