using Microsoft.EntityFrameworkCore;
using Product.Data.Constants;
using Shared.DbSeeding;
using ShoppingCart.Data;
using ShoppingCart.HostedServices;
using ShoppingCart.Integration;
using System.Reflection;
using static Grpc.Discount.DiscountGrpcService;

namespace ShoppingCart.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();

        AddGrpcTypedClients(services, configuration);

        AddDataAccessServices(services, configuration);

        AddApplicationServices(services, configuration);

        AddUtilitiesServices(services, configuration);

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

    private static void AddGrpcTypedClients(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<DiscountGrpcServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:5003");
        });

    }

    private static void AddApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDiscountIntegrationService,  DiscountIntegrationService>();
    }

    private static void AddUtilitiesServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(options =>
        {
            options.AddMaps(Assembly.GetExecutingAssembly());
        });
    }
}
