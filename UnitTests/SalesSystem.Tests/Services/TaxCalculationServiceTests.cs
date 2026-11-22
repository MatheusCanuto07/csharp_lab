using SalesSystem.Models;
using SalesSystem.Services;

namespace SalesSystem.Tests.Services;

public class TaxCalculationServiceTests
{
    private readonly TaxCalculationService _sut = new();
    
    [Theory]
    // Cada InlineData é uma execução separada
    [InlineData(Region.Southeast, 0.18)]
    [InlineData(Region.South, 0.17)]
    [InlineData(Region.Northeast, 0.12)]
    [InlineData(Region.North, 0.10)]
    [InlineData(Region.Midwest, 0.15)]
    [InlineData(Region.FreeTradeZone, 0.00)]
    public void GetTaxRate_ReturnsExpectedRateForRegion(Region region, decimal expectedRate)
    {
        var rate = _sut.GetTaxRate(region);

        Assert.Equal(expectedRate, rate);
    }
    
    // Teste simples, um cenário, sem parametro
    [Fact]
    public void CalculateTotal_AppliesRegionalTaxToTaxableItems()
    {
        var cart = new ShoppingCart
        {
            Region = Region.Southeast,
            Items =
            [
                new CartItem { Name = "Phone", UnitPrice = 1000m, Quantity = 1, IsTaxExempt = false }
            ]
        };

        var result = _sut.CalculateTotal(cart);

        Assert.Equal(1000m, result.Subtotal);
        Assert.Equal(180m, result.TaxAmount);
        Assert.Equal(0.18m, result.TaxRate);
        Assert.Equal(1180m, result.Total);
        Assert.Equal(Region.Southeast, result.Region);
    }

    [Fact]
    public void CalculateTotal_SkipsTaxForExemptItems()
    {
        var cart = new ShoppingCart
        {
            Region = Region.South,
            Items =
            [
                new CartItem { Name = "Book", UnitPrice = 40m, Quantity = 2, IsTaxExempt = true },
                new CartItem { Name = "Cable", UnitPrice = 20m, Quantity = 1, IsTaxExempt = false }
            ]
        };

        var result = _sut.CalculateTotal(cart);

        // Subtotal: (40*2) + 20 = 100; tax only on cable: 20 * 0.17 = 3.40
        Assert.Equal(100m, result.Subtotal);
        Assert.Equal(3.40m, result.TaxAmount);
        Assert.Equal(103.40m, result.Total);
    }

    [Fact]
    public void CalculateTotal_ZeroRateRegion_ProducesNoTax()
    {
        var cart = new ShoppingCart
        {
            Region = Region.FreeTradeZone,
            Items =
            [
                new CartItem { Name = "Monitor", UnitPrice = 500m, Quantity = 1, IsTaxExempt = false }
            ]
        };

        var result = _sut.CalculateTotal(cart);

        Assert.Equal(500m, result.Subtotal);
        Assert.Equal(0m, result.TaxAmount);
        Assert.Equal(0m, result.TaxRate);
        Assert.Equal(500m, result.Total);
    }

    [Fact]
    public void CalculateTotal_EmptyCart_ReturnsZeros()
    {
        var cart = new ShoppingCart
        {
            Region = Region.Northeast,
            Items = []
        };

        var result = _sut.CalculateTotal(cart);

        Assert.Equal(0m, result.Subtotal);
        Assert.Equal(0m, result.TaxAmount);
        Assert.Equal(0m, result.Total);
        Assert.Equal(0.12m, result.TaxRate);
    }

    [Fact]
    public void CalculateTotal_NullCart_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.CalculateTotal(null!));
    }

    [Fact]
    public void CalculateTotal_IgnoresInvalidLineItems()
    {
        var cart = new ShoppingCart
        {
            Region = Region.North,
            Items =
            [
                new CartItem { Name = "Broken", UnitPrice = 10m, Quantity = 0, IsTaxExempt = false },
                new CartItem { Name = "Negative", UnitPrice = -5m, Quantity = 2, IsTaxExempt = false },
                new CartItem { Name = "Valid", UnitPrice = 50m, Quantity = 2, IsTaxExempt = false }
            ]
        };

        var result = _sut.CalculateTotal(cart);

        Assert.Equal(100m, result.Subtotal);
        Assert.Equal(10m, result.TaxAmount); // 100 * 0.10
        Assert.Equal(110m, result.Total);
    }
}
