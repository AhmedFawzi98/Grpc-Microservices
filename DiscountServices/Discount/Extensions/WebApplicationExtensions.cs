using Discount.GrpcServices;

namespace Discount.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.MapGrpcService<DiscountService>();

        return app;
    }
}
