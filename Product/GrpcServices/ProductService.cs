using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Product;
using Product.Data;
using Product.Helpers.GrpcHelpers;
using static Grpc.Product.ProductGrpcService;
using ProductResponse = Grpc.Product.Product;
namespace Product.GrpcServices;

public class ProductService(ProductDbContext productDbContext) : ProductGrpcServiceBase
{
    public override Task<Empty> Test(Empty request, ServerCallContext context)
    {
        return base.Test(request, context);
    }

    public override async Task<Grpc.Product.Product> AddProduct(AddProductRequest request, ServerCallContext context)
    {
        var product = await productDbContext.Products.FindAsync(request.Product.Id);
        if (product == null)
        {
            //throw rpc exception
        }

        var productResponse = new ProductResponse()
        {
            Id = product!.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ProductStatus = product.ProductStatus.ToGrpc(),
            CreatedDate = Timestamp.FromDateTime(product.CreatedDate),
        };

        return productResponse;
    }
}
