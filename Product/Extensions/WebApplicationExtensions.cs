using Product.GrpcServices;

namespace Product.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.MapGrpcService<ProductService>();

        return app;
    }
}
