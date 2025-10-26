using AutoMapper;
using ProductResponse = Grpc.Product.Product;
using ProductEntity = Product.Models.Product;
using Product.Helpers.GrpcHelpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Product;

namespace Product.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductEntity, ProductResponse>()
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus.ToGrpc()))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimestamp()));

        CreateMap<AddProductRequest, ProductEntity>()
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => Enums.ProductStatus.Instock))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductRequest, ProductEntity>();
    }

}
