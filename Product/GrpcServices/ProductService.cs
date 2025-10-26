using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Product;
using Microsoft.EntityFrameworkCore;
using Product.Data;
using Product.Helpers.GrpcHelpers;
using static Grpc.Product.ProductGrpcService;
using ProductResponse = Grpc.Product.Product;
using ProductEntity = Product.Models.Product;
using AutoMapper;
namespace Product.GrpcServices;

public class ProductService(ProductDbContext productDbContext, IMapper mapper) : ProductGrpcServiceBase
{
    public override Task<Empty> Test(Empty request, ServerCallContext context)
    {
        return base.Test(request, context);
    }

    public override async Task<ProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var product = await productDbContext.Products.FindAsync(request.Id);
        if (product == null)
        {
            //throw rpc exception
        }

        var productResponse = mapper.Map<ProductResponse>(product);

        return productResponse;
    }

    public override async Task GetAllProducts(
        GetAllProductsRequest request,
        IServerStreamWriter<ProductResponse> responseStream,
        ServerCallContext context)
    {
        //for simplicity, no filtering/pagination paramters sent in GetAllProductsRequest
        IAsyncEnumerable<ProductEntity> products = productDbContext.Products.AsAsyncEnumerable();

        await foreach(var product in products)
        {
            var productResponse = mapper.Map<ProductResponse>(product);

            await responseStream.WriteAsync(productResponse);
        }
    }

    public override async Task<ProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
    {
        var product = mapper.Map<ProductEntity>(request);

        productDbContext.Products.Add(product);
        await productDbContext.SaveChangesAsync();

        var productResponse = mapper.Map<ProductResponse>(product);

        return productResponse;
    }

    public override async Task<ProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        var product = await productDbContext.Products.FindAsync(request.Id);
        if(product == null)
        {
            //throw rpc exception
        }

        mapper.Map(request, product);

        await productDbContext.SaveChangesAsync();

        var productResponse = mapper.Map<ProductResponse>(product);

        return productResponse;
    }

    public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var product = await productDbContext.Products.FindAsync(request.Id);
        if (product == null)
        {
            //throw rpc exception
        }

        productDbContext.Products.Remove(product!);
        var deletedCount = await productDbContext.SaveChangesAsync();

        var deleteProductResponse = new DeleteProductResponse()
        {
            Success = deletedCount > 0,
        };

        return deleteProductResponse;
    }
}
