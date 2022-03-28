using Api.Controllers;
using Api.Requests.Product;
using Api.Requests.Validations;
using Api.Service;
using Domain.Repositories.Interfaces;
using FluentValidation;
using Mysql.Context;
using Mysql.Identity;
using Mysql.Repositories;

namespace Api.ApplicationBuilder;

public static class ExtensionBuildDependencyInjection
{
    internal static void InjectDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DataContext>();
        builder.Services.AddScoped<DataContext, DataContext>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IUserRepository<ApplicationUser>, UserRepository>();
        builder.Services.AddScoped<RefreshTokenRepository>();

        builder.Services.AddTransient<IValidator<ProductRequest>, ProductValidator>();
        builder.Services.AddTransient<IValidator<AuthController.RegisterRequest>, RegistrationValidator>();

        builder.Services.AddScoped<JwtTokenService>();
    }
}
