using Microsoft.AspNetCore.Mvc;
using Moq;
using SalesSystem.Controllers;
using SalesSystem.Models;
using SalesSystem.Services;

namespace SalesSystem.Tests.Controllers;

public class CartControllerTests
{
    private readonly Mock<ITaxCalculationService> _taxServiceMock = new();
    private readonly CartController _sut;

    public CartControllerTests()
    {
        _sut = new CartController(_taxServiceMock.Object);
    }

    [Fact]
    public void CalculateTotal_ValidCart_ReturnsOkWithServiceResult()
    {
        // ARRANGE (preparar): primeira etapa do padrão AAA.
        // Montamos o cenário completo do teste: o carrinho de entrada (ShoppingCart)
        // e o CartTotalResult que "fingimos" que o serviço de imposto devolveria.
        // Em testes de controlador, isolamos a dependência real (TaxCalculationService)
        // usando um mock (dublê de teste) do ITaxCalculationService via biblioteca Moq.
        // Assim avaliamos só o CartController, sem executar a lógica real de imposto.
        var cart = new ShoppingCart
        {
            Region = Region.Midwest,
            Items =
            [
                new CartItem { Name = "Desk", UnitPrice = 200m, Quantity = 1, IsTaxExempt = false }
            ]
        };

        var expected = new CartTotalResult
        {
            Subtotal = 200m,
            TaxAmount = 30m,
            TaxRate = 0.15m,
            Total = 230m,
            Region = Region.Midwest
        };

        // Ainda no ARRANGE — configuração do Moq (dublê / test double):
        // Setup(...): "quando alguém chamar CalculateTotal passando este cart..."
        _taxServiceMock
            .Setup(s => s.CalculateTotal(cart))
            // Returns(expected): "...então devolva este CartTotalResult configurado".
            // Em linguagem simples: esta linha "ensina" o mock a responder.
            // O mock substitui (é um stand-in / dublê de) o ITaxCalculationService real.
            // Quando o controlador chamar CalculateTotal, NÃO roda TaxCalculationService
            // de verdade — recebe imediatamente o 'expected' que definimos acima.
            // Por isso o teste do controlador fica isolado e previsível.
            .Returns(expected);

        // ACT (agir): segunda etapa do AAA — executamos o método sob teste.
        // Chamamos CalculateTotal no controlador (_sut = System Under Test),
        // que internamente deve delegar ao ITaxCalculationService (nosso mock).
        var actionResult = _sut.CalculateTotal(cart);

        // ASSERT (verificar): terceira etapa do AAA — checamos o resultado.
        // Confirmamos HTTP 200 (OkObjectResult), o corpo CartTotalResult,
        // e com Verify que o mock foi chamado exatamente uma vez com o cart.
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var body = Assert.IsType<CartTotalResult>(okResult.Value);
        Assert.Equal(230m, body.Total);
        Assert.Equal(30m, body.TaxAmount);
        _taxServiceMock.Verify(s => s.CalculateTotal(cart), Times.Once);
    }

    [Fact]
    public void CalculateTotal_NullCart_ReturnsBadRequest()
    {
        var actionResult = _sut.CalculateTotal(null!);

        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        _taxServiceMock.Verify(s => s.CalculateTotal(It.IsAny<ShoppingCart>()), Times.Never);
    }

    [Fact]
    public void CalculateTotal_EmptyCart_DelegatesToServiceAndReturnsOk()
    {
        var cart = new ShoppingCart { Region = Region.Southeast, Items = [] };
        var expected = new CartTotalResult
        {
            Subtotal = 0m,
            TaxAmount = 0m,
            TaxRate = 0.18m,
            Total = 0m,
            Region = Region.Southeast
        };

        _taxServiceMock
            .Setup(s => s.CalculateTotal(cart))
            .Returns(expected);

        var actionResult = _sut.CalculateTotal(cart);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var body = Assert.IsType<CartTotalResult>(okResult.Value);
        Assert.Equal(0m, body.Total);
        _taxServiceMock.Verify(s => s.CalculateTotal(cart), Times.Once);
    }

    [Fact]
    public void CalculateTotal_ZeroTaxRate_ReturnsTotalEqualToSubtotal()
    {
        var cart = new ShoppingCart
        {
            Region = Region.FreeTradeZone,
            Items =
            [
                new CartItem { Name = "Laptop", UnitPrice = 3000m, Quantity = 1, IsTaxExempt = false }
            ]
        };

        var expected = new CartTotalResult
        {
            Subtotal = 3000m,
            TaxAmount = 0m,
            TaxRate = 0m,
            Total = 3000m,
            Region = Region.FreeTradeZone
        };

        _taxServiceMock
            .Setup(s => s.CalculateTotal(It.IsAny<ShoppingCart>()))
            .Returns(expected);

        var actionResult = _sut.CalculateTotal(cart);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var body = Assert.IsType<CartTotalResult>(okResult.Value);
        Assert.Equal(0m, body.TaxAmount);
        Assert.Equal(body.Subtotal, body.Total);
    }
}
