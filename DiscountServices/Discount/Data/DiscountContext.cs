using DiscountEntity = Discount.Data.Models.Discount;

namespace Discount.Data;

public class DiscountContext
{
    public readonly List<DiscountEntity> Discounts = new()
    {
        new DiscountEntity() {Id = 1, Code = "CODE_100", Amount = 100m},
        new DiscountEntity() {Id = 2, Code = "CODE_200", Amount = 200m},
        new DiscountEntity() {Id = 3, Code = "CODE_300", Amount = 300m},
    };
}
