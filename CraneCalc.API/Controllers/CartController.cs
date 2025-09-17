using CraneCalc.Application.Features.Cart.Commands.DeleteCart;
using CraneCalc.Application.Features.Cart.Commands.FormCart;
using CraneCalc.Application.Features.Cart.Commands.ModerateCart;
using CraneCalc.Application.Features.Cart.Commands.UpdateCart;
using CraneCalc.Application.Features.Cart.Queries.GetCart;
using CraneCalc.Application.Features.Cart.Queries.GetCartInfo;
using CraneCalc.Application.Features.Cart.Queries.GetFilteredCarts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpGet("info")]
    public async Task<IActionResult> GetCartInformation( CancellationToken ct)
    {
        var query = new GetCartInformationQuery();
        var result = await mediator.Send(query, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredCarts([FromQuery] GetFilteredCartsQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        return Ok(result);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCart([FromQuery] GetCartQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCart(
        [FromBody] UpdateCartCommand request,
        CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPut("form")]
    public async Task<IActionResult> FormCart([FromQuery] FormCartCommand query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPut("moderate")]
    public async Task<IActionResult> ModerateCart([FromQuery] ModerateCartCommand query, CancellationToken ct)
    {
        var cart = await mediator.Send(query, ct);
        
        if(cart == null)
            return NotFound();
        
        return Ok(cart);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCart([FromQuery] DeleteCartCommand query, CancellationToken ct)
    {
        await mediator.Send(query, ct);

        return NoContent();
    }
}