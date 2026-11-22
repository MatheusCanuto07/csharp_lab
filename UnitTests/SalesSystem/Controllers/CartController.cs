using Microsoft.AspNetCore.Mvc;
using SalesSystem.Models;
using SalesSystem.Services;

namespace SalesSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ITaxCalculationService _taxCalculationService;

    public CartController(ITaxCalculationService taxCalculationService)
    {
        _taxCalculationService = taxCalculationService;
    }

    /// <summary>
    /// Receives a shopping cart and returns the total with regional tax applied.
    /// </summary>
    [HttpPost("total")]
    [ProducesResponseType(typeof(CartTotalResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CartTotalResult> CalculateTotal([FromBody] ShoppingCart cart)
    {
        if (cart is null)
        {
            return BadRequest("Shopping cart is required.");
        }
        
                
        var result = _taxCalculationService.CalculateTotal(cart);
        return Ok(result);
    }
}
