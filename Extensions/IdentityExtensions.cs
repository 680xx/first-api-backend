using System.Text;
using first_api_backend.Context;
using first_api_backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace first_api_backend.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
        return services;
    }
    
    public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 1;
            options.User.RequireUniqueEmail = true;
        });
        return services;
    }
    
    // Auth = Authentication + Authorization
    public static IServiceCollection AddIdentityAuth(
        this IServiceCollection services,
        IConfiguration config)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(y =>
            {
                y.SaveToken = false;
                y.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            config["AppSettings:JWTSecret"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        // Kräver att man är inloggad för att nå endpoints
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("HasLibraryID", policy => policy.RequireClaim("LibraryID"));
            options.AddPolicy("FemalesOnly", policy => policy.RequireClaim("Gender", "Female"));
            options.AddPolicy("MaleOnly", policy => policy.RequireClaim("Gender", "Male"));
            options.AddPolicy("Under10", policy => policy.RequireAssertion(context => 
                Int32.Parse(context.User.Claims.First(x => x.Type=="Age").Value)<10));
                    
        });
        return services;
    }
    
    public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app; 
    }
    
}