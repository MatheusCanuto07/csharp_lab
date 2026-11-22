using SalesSystem.Models;

namespace SalesSystem.Services;

public interface ITaxCalculationService
{
    decimal GetTaxRate(Region region);
    CartTotalResult CalculateTotal(ShoppingCart cart);
}
