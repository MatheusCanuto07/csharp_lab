namespace SalesSystem.Models;

public class ShoppingCart
{
    public Region Region { get; set; }
    public List<CartItem> Items { get; set; } = new();
}
