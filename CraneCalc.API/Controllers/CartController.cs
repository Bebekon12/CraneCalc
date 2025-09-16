using CraneCalc.Application.Dtos.Request;
using CraneCalc.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController(ICartRepository repository) : ControllerBase
{
    [HttpGet("info")]
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

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredCarts(DateTime from, DateTime before, CancellationToken ct)
    {
        var carts = await repository.GetFilteredCartsAsync(from, before, ct);
        
        return Ok(carts);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCart(Guid id, CancellationToken ct)
    {
        var cart = await repository.GetCartByIdAsync(id, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(cart);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCart(
        Guid cartId,
        [FromBody] UpdateCartRequest request,
        CancellationToken ct)
    {
        var cart = await repository.UpdateCartAsync(cartId, request, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(cart);
    }

    [HttpPut("{cartId:guid}/form")]
    public async Task<IActionResult> FormCart(Guid cartId, CancellationToken ct)
    {
        var cart = await repository.FormCartAsync(cartId, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(cart);
    }

    [HttpPut("{cartId:guid}/moderate")]
    public async Task<IActionResult> ModerateCart(Guid cartId, bool isApproved, CancellationToken ct)
    {
        var cart = await repository.ModerateCartAsync(cartId, 1, isApproved, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(cart);
    }

    [HttpDelete("delete/{cartId:guid}")]
    public async Task<IActionResult> DeleteCart(Guid cartId, CancellationToken ct)
    {
        await repository.DeleteCartAsync(cartId, 1, ct);

        return NoContent();
    }
    
}