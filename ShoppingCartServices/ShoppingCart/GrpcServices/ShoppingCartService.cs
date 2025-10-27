using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.ShoppingCart;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Data.Models;
using ShoppingCart.Integration;
using static Grpc.ShoppingCart.ShoppingCartGrpcService;
using ShoppingCartEntity = ShoppingCart.Data.Models.ShoppingCart;
using ShoppingCartItemEntity = ShoppingCart.Data.Models.ShoppingCartItem;
using ShoppingCartResponse = Grpc.ShoppingCart.ShoppingCart;

namespace ShoppingCart.GrpcServices;

public class ShoppingCartService(
    ShoppingCartDbContext shoppingCartDbContext,
    IDiscountIntegrationService discountIntegrationService,
    IMapper mapper)
    : ShoppingCartGrpcServiceBase
{
    public override async Task<ShoppingCartResponse> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await shoppingCartDbContext
            .ShoppingCarts
            .Include(sc => sc.Items)
            .FirstOrDefaultAsync(sc => sc.UserName == request.Username);

        if(shoppingCart == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"shopping cart with username: {request.Username} does not exist."));
        }

        var shoppingCartResponse = mapper.Map<ShoppingCartResponse>(shoppingCart);

        return shoppingCartResponse;
    }

    public override async Task<ShoppingCartResponse> AddShoppingCart(AddShoppingCartRequest request, ServerCallContext context)
    {
        var isExist = await shoppingCartDbContext.ShoppingCarts.AnyAsync(sc => sc.UserName == request.Username);
        if(isExist)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, $"shopping cart with username: {request.Username} already exist."));
        }

        var shoppingCart = mapper.Map<ShoppingCartEntity>(request);

        if (request.ShoppingCartItems != null)
        {
            await AddShoppingCartItemsAsync(shoppingCart, request);
        }        

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
        ShoppingCartEntity shoppingCart = null;
        await foreach (var addItemIntoShoppingCartRequest in requestStream.ReadAllAsync())
        {
            if (addItemIntoShoppingCartRequest.ShoppingCartItem == null)
                continue;

            shoppingCart = shoppingCartDbContext.ShoppingCarts
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

                var discountDto = await discountIntegrationService.GetDiscountAsync(addItemIntoShoppingCartRequest.DiscountCode);
                shoppingCartItemEntity.Price = Math.Max(0, shoppingCartItemEntity.Price - discountDto.Amount);

                shoppingCart!.Items.Add(shoppingCartItemEntity);
            }
            else
            {
                shoppingCartItemInShoppingCart.Quantity += shoppingCartItemToAdd.Quantity;
            }
        }

        Console.WriteLine(shoppingCart.Items.Count);
        await shoppingCartDbContext.SaveChangesAsync();
        Console.WriteLine(shoppingCart.Items.Count);

        return new AddItemIntoShoppingCartResponse()
        {
            Sucess = true,
        };
    }

    private async Task AddShoppingCartItemsAsync(ShoppingCartEntity shoppingCart, AddShoppingCartRequest request)
    {
        var items = new List<ShoppingCartItemToAdd>();

        foreach (var itemToAdd in request.ShoppingCartItems)
        {
            var existingItemOfSameType = items.FirstOrDefault(i => i.ProductId == itemToAdd.ProductId && i.Color == itemToAdd.Color);
            if (existingItemOfSameType == null)
            {
                items.Add(itemToAdd);
            }
            else
            {
                existingItemOfSameType.Quantity += itemToAdd.Quantity;
            }
        }

        var shoppingCartItemEntities = mapper.Map<List<ShoppingCartItemEntity>>(items);

        var discountDto = await discountIntegrationService.GetDiscountAsync(request.DiscountCode);

        foreach (var shoppingCartItemEntity in shoppingCartItemEntities)
        {
            shoppingCartItemEntity.Price = Math.Max(0, shoppingCartItemEntity.Price - discountDto.Amount);
        }

        shoppingCart.Items.AddRange(shoppingCartItemEntities);

        shoppingCartDbContext.ShoppingCarts.Add(shoppingCart);
    }

}
