using first_api_backend.Context;
using Microsoft.EntityFrameworkCore;

namespace first_api_backend.Extensions;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        return services;
    }
}