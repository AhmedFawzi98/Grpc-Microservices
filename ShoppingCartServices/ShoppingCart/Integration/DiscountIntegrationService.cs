using Grpc.Discount;
using ShoppingCart.Dtos;
using static Grpc.Discount.DiscountGrpcService;

namespace ShoppingCart.Integration
{
    public class DiscountIntegrationService(DiscountGrpcServiceClient discountGrpcServiceClient) : IDiscountIntegrationService
    {
        public async Task<DiscountDto> GetDiscountAsync(string discountCode)
        {
            var request = new GetDiscountRequest() { Code = discountCode };

            var discountResponse = await discountGrpcServiceClient.GetDiscountAsync(request);

            return new DiscountDto(discountResponse.Id, discountResponse.Code, discountResponse.Amount);
        }
    }
}
