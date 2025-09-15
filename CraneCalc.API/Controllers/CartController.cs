using CraneCalc.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartRepository repository) : ControllerBase
{
    [HttpGet("cart-info")]
    public async Task<IActionResult> GetCartInformation(CancellationToken ct)
    {
        var cart = await repository.GetCartByUserIdAsync(1, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(new
        {
            cart.Id,
            Quantity = cart.CartCargo.Count
        });
    }

    [HttpGet("filtered-carts")]
    public async Task<IActionResult> GetFilteredCarts(DateTime from, DateTime before, CancellationToken ct)
    {
        var carts = await repository.GetFilteredCartsAsync(from, before, ct);
        
        return Ok(carts);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCart(Guid id, CancellationToken ct)
    {
        var cart = await repository.GetCartByIdAsync(id, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(cart);
    }
}