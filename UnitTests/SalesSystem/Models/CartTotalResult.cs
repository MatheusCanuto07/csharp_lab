namespace SalesSystem.Models;

public class CartTotalResult
{
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public Region Region { get; set; }
}
