using CraneCalc.Application.Features.CartCargo.Commands.DeleteCargoInCart;
using CraneCalc.Application.Features.CartCargo.Commands.UpdateCartCargo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/cart-cargo")]
public class CartCargoController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteCargoInCart([FromQuery] DeleteCargoInCartCommand query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok();
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateCartCargo([FromQuery] UpdateCartCargoCommand query,
        CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok();
    }
}