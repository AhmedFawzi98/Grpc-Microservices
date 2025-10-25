using Microsoft.EntityFrameworkCore;
using Product.Data;
using Product.Data.Constants;
using Product.HostedServices;
using Shared.DbSeeding;

namespace Product.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();

        AddDataAccessServices(services, configuration);

        return services;
    }

    private static void AddDataAccessServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseInMemoryDatabase(DatabaseConstants.ProductDatabaseName);
        });

        services.AddScoped<IDbSeeder, ProductDbSeeder>();
        services.AddHostedService<DbInitalizer>();
    }
}
