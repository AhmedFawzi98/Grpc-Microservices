using ShoppingCart.Dtos;

namespace ShoppingCart.Integration;

public interface IDiscountIntegrationService
{
    Task<DiscountDto> GetDiscountAsync(string discountCode);
}
