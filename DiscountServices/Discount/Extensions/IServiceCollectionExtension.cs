
using Discount.Data;

namespace Discount.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();

        services.AddSingleton<DiscountContext>();

        return services;
    }
}
