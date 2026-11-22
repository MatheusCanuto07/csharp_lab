namespace SalesSystem.Models;

public class CartItem
{
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public bool IsTaxExempt { get; set; }
}
