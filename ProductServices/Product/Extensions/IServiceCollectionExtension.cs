using Microsoft.EntityFrameworkCore;
using Product.Data;
using Product.Data.Constants;
using Product.HostedServices;
using Shared.DbSeeding;
using Shared.Interceptors;
using System.Reflection;

namespace Product.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddGrpc(options =>
        {
            if(env.IsDevelopment())
                options.EnableDetailedErrors = true;
    
            options.Interceptors.Add<ServerGrpcExceptionsInterceptor>();
            options.Interceptors.Add<ServerLoggingInterceptor>();
        });

        AddDataAccessServices(services, configuration);

        services.AddAutoMapper(options =>
        {
            options.AddMaps(Assembly.GetExecutingAssembly());
        });

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
