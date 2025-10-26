namespace ShoppingCart.Data.Models;

public class ShoppingCart
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

    public ShoppingCart()
    {
    }

    public ShoppingCart(string userName)
    {
        UserName = userName;
    }

    public decimal TotalPrice
    {
        get
        {
            decimal totalprice = 0m;
            foreach (var item in Items)
            {
                totalprice += item.Price * item.Quantity;
            }
            return totalprice;
        }
    }
}