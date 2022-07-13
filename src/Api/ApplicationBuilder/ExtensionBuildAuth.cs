using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Mysql.Context;
using Mysql.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Api.ApplicationBuilder;

public static class ExtensionBuildAuth
{
    public static void BuildAuth(this WebApplicationBuilder builder)
    {
        //Disable default claim names by Identity
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Append(new KeyValuePair<string, string>("roles", "roles"));

        builder.Services
            .AddHttpContextAccessor()
            .AddAuthorization()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(0),
                    RequireExpirationTime = true,
                    RoleClaimType = "roles", //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/claims?view=aspnetcore-6.0
                };
            });

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.RequireUniqueEmail = true;

            // SignIn settings
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<DataContext>();
    }

    internal static void UseAuth(this WebApplication app)
    {
        app.UseAuthentication();

        app.UseAuthorization();
    }
}
