using AutoMapper;
using Grpc.Core;
using Grpc.ShoppingCart;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using static Grpc.ShoppingCart.ShoppingCartGrpcService;
using ShoppingCartResponse = Grpc.ShoppingCart.ShoppingCart;
using ShoppingCartEntity = ShoppingCart.Data.Models.ShoppingCart;
using ShoppingCartItemEntity = ShoppingCart.Data.Models.ShoppingCartItem;

namespace ShoppingCart.GrpcServices;

public class ShoppingCartService(ShoppingCartDbContext shoppingCartDbContext, IMapper mapper) : ShoppingCartGrpcServiceBase
{
    public override async Task<Grpc.ShoppingCart.ShoppingCart> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await shoppingCartDbContext
            .ShoppingCarts
            .FirstOrDefaultAsync(sc => sc.UserName == request.Username);

        if(shoppingCart == null)
        {
            //throw rpc exception
        }

        var shoppingCartResponse = mapper.Map<ShoppingCartResponse>(shoppingCart);

        return shoppingCartResponse;
    }

    public override async Task<ShoppingCartResponse> AddShoppingCart(AddShoppingCartRequest request, ServerCallContext context)
    {
        var isExist = await shoppingCartDbContext.ShoppingCarts.AnyAsync(sc => sc.UserName == request.Username);
        if(isExist)
        {
            //throw rpcexception 
        }

        var shoppingCart = mapper.Map<ShoppingCartEntity>(request);

        var items = new List<ShoppingCartItemToAdd>();
        foreach (var itemToAdd in request.ShoppingCartItems)
        {
            var existingItemOfSameType = items.FirstOrDefault(i => i.ProductId == itemToAdd.ProductId && i.Color == itemToAdd.Color);
            if(existingItemOfSameType == null)
            {
                items.Add(itemToAdd);
            }
            else
            {
                existingItemOfSameType.Quantity += itemToAdd.Quantity;
            }
        }

        var shoppingCartItemEntities = mapper.Map<List<ShoppingCartItemEntity>>(items);

        //todo: instead of hardcoded value, call discount service (gRPC call) to get discount for each item type
        var discount = 100m;
        foreach (var shoppingCartItemEntity in shoppingCartItemEntities)
        {
            shoppingCartItemEntity.Price -= discount;
        }

        shoppingCart.Items.AddRange(shoppingCartItemEntities);

        shoppingCartDbContext.ShoppingCarts.Add(shoppingCart);  
        await shoppingCartDbContext.SaveChangesAsync();

        var shoppingCartResponse = mapper.Map<ShoppingCartResponse>(shoppingCart);

        return shoppingCartResponse;
    }

    public override async Task<DeleteItemFromShoppingCartResponse> DeleteItemFromShoppingCart(DeleteItemFromShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await shoppingCartDbContext
            .ShoppingCarts
            .FirstOrDefaultAsync(sc => sc.UserName == request.Username);

        if (shoppingCart == null)
        {
            // throw rpc exception 
        }

        var shoppingCartItem = shoppingCart.Items.FirstOrDefault(i => i.Id == request.ShoppingCartItemId);
        if (shoppingCartItem == null)
        {
            // throw rpc exception 
        }

        shoppingCartDbContext.ShoppingCartItems.Remove(shoppingCartItem!);
        var deletedCount = await shoppingCartDbContext.SaveChangesAsync();

        return new DeleteItemFromShoppingCartResponse()
        {
            Sucess = deletedCount > 0
        };
    }

    public override async Task<AddItemIntoShoppingCartResponse> AddItemIntoShoppingCart(IAsyncStreamReader<AddItemIntoShoppingCartRequest> requestStream, ServerCallContext context)
    {
        await foreach (var addItemIntoShoppingCartRequest in requestStream.ReadAllAsync())
        {
            var shoppingCart = shoppingCartDbContext.ShoppingCarts
                    .Local
                    .FirstOrDefault(sc => sc.UserName == addItemIntoShoppingCartRequest.Username) 
                    ?? await shoppingCartDbContext.ShoppingCarts
                        .Include(sc => sc.Items)
                        .FirstOrDefaultAsync(sc => sc.UserName == addItemIntoShoppingCartRequest.Username);
            
            if (shoppingCart == null)
            {
                // throw rpc exception 
            }

            var shoppingCartItemToAdd = addItemIntoShoppingCartRequest.ShoppingCartItem;

            var shoppingCartItemInShoppingCart = shoppingCart!
                .Items
                .FirstOrDefault(i => i.ProductId == shoppingCartItemToAdd.ProductId
                    && i.Color == shoppingCartItemToAdd.Color);

            if (shoppingCartItemInShoppingCart == null)
            {
                var shoppingCartItemEntity = mapper.Map<ShoppingCartItemEntity>(shoppingCartItemToAdd);

                //todo: instead of hardcoded value, call discount service (gRPC call) to get discount for item
                var discount = 100m;
                shoppingCartItemEntity.Price -= discount;

                shoppingCart!.Items.Add(shoppingCartItemEntity);
            }
            else
            {
                shoppingCartItemInShoppingCart.Quantity += shoppingCartItemToAdd.Quantity;
            }
        }

        await shoppingCartDbContext.SaveChangesAsync();

        return new AddItemIntoShoppingCartResponse()
        {
            Sucess = true,
        };
    }
}
