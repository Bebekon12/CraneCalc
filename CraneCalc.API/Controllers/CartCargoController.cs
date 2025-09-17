using CraneCalc.Application.Interfaces;
using CraneCalc.Application.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/cart-cargo")]
public class CartCargoController(ICartCargoRepository repository) : ControllerBase
{
    [HttpDelete]
    public async Task<IActionResult> DeleteCargoInCart(Guid cartId, Guid cargoId, CancellationToken ct)
    {
        var result = await repository.DeleteCargoInCartAsync(cartId, cargoId, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCartCargo(Guid cartId, Guid cargoId, string safetyComment,
        CancellationToken ct)
    {
        var result = await repository.UpdateCargoInCartAsync(cartId, cargoId, safetyComment, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok();
    }
}