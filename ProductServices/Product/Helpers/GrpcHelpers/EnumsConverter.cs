namespace Product.Helpers.GrpcHelpers;

public static class EnumsConverter
{
    public static Grpc.Product.ProductStatus ToGrpc(this Enums.ProductStatus status)
        => (Grpc.Product.ProductStatus)status;

    public static Enums.ProductStatus ToDomain(this Grpc.Product.ProductStatus status)
        => (Enums.ProductStatus)status;
}
