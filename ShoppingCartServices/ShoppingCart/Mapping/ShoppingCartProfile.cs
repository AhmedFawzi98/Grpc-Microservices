using AutoMapper;

namespace ShoppingCart.Mapping;

public class ShoppingCartProfile : Profile
{
    public ShoppingCartProfile()
    {
        CreateMap<Data.Models.ShoppingCartItem, Grpc.ShoppingCart.ShoppingCartItem>();
        CreateMap<Data.Models.ShoppingCart, Grpc.ShoppingCart.ShoppingCart>()
            .ForMember(dest => dest.ShoppingCartItems, opt => opt.MapFrom(src => src.Items));

        CreateMap<Grpc.ShoppingCart.ShoppingCartItemToAdd, Data.Models.ShoppingCartItem>();
        CreateMap<Grpc.ShoppingCart.AddShoppingCartRequest, Data.Models.ShoppingCart>()
            .ForMember(dest => dest.Items, opt => opt.Ignore());
    }
    
}
