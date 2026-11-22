using SalesSystem.Models;

namespace SalesSystem.Services;

/// <summary>
/// Applies regional tax rates to a shopping cart.
/// Exempt items and zero-rate regions contribute no tax.
/// </summary>
public class TaxCalculationService : ITaxCalculationService
{
    private static readonly IReadOnlyDictionary<Region, decimal> Rates =
        new Dictionary<Region, decimal>
        {
            [Region.Southeast] = 0.18m,
            [Region.South] = 0.17m,
            [Region.Northeast] = 0.12m,
            [Region.North] = 0.10m,
            [Region.Midwest] = 0.15m,
            [Region.FreeTradeZone] = 0.00m
        };

    public decimal GetTaxRate(Region region) =>
        Rates.TryGetValue(region, out var rate) ? rate : 0m;

    public CartTotalResult CalculateTotal(ShoppingCart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        var items = cart.Items ?? new List<CartItem>();
        var taxRate = GetTaxRate(cart.Region);

        decimal subtotal = 0m;
        decimal taxAmount = 0m;

        foreach (var item in items)
        {
            if (item.Quantity <= 0 || item.UnitPrice < 0)
            {
                continue;
            }

            var lineTotal = item.UnitPrice * item.Quantity;
            subtotal += lineTotal;

            if (!item.IsTaxExempt && taxRate > 0m)
            {
                taxAmount += lineTotal * taxRate;
            }
        }

        return new CartTotalResult
        {
            Subtotal = subtotal,
            TaxAmount = RoundMoney(taxAmount),
            TaxRate = taxRate,
            Total = RoundMoney(subtotal + taxAmount),
            Region = cart.Region
        };
    }

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
