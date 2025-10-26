using Microsoft.EntityFrameworkCore;
using Product.Data.Constants;
using Shared.DbSeeding;
using ShoppingCart.Data;
using ShoppingCart.HostedServices;
using System.Reflection;

namespace ShoppingCart.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();

        AddDataAccessServices(services, configuration);

        services.AddAutoMapper(options =>
        {
            options.AddMaps(Assembly.GetExecutingAssembly());
        });

        return services;
    }

    private static void AddDataAccessServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShoppingCartDbContext>(options =>
        {
            options.UseInMemoryDatabase(DatabaseConstants.ShoppingCartDatabaseName);
        });

        services.AddScoped<IDbSeeder, ShoppingCartDbSeeder>();
        services.AddHostedService<DbInitalizer>();
    }
}
