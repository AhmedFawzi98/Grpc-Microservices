using Discount.Data;
using Grpc.Core;
using Grpc.Discount;
using static Grpc.Discount.DiscountGrpcService;

namespace Discount.GrpcServices;

public class DiscountService(DiscountContext discountContext) : DiscountGrpcServiceBase
{
    public override Task<Grpc.Discount.Discount> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var discount = discountContext.Discounts.FirstOrDefault(d => d.Code == request.Code);
        if (discount == null)
        {
            //throw rpc excpetion
        }

        var discountResponse = new Grpc.Discount.Discount()
        {
            Id = discount!.Id,
            Code = discount.Code,
            Amount = discount.Amount,
        };
        return Task.FromResult(discountResponse);
    }
}
