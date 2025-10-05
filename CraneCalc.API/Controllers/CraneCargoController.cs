using CraneCalc.Application.Features.CraneCargo.Commands.DeleteCargoInCraneOrder;
using CraneCalc.Application.Features.CraneCargo.Commands.UpdateCraneCargo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/crane-cargo")]
public class CraneCargoController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteCargoInCraneOrder([FromQuery] DeleteCargoInCraneOrderCommand query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok();
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateCraneOrderCargo([FromQuery] UpdateCraneOrderCargoCommand query,
        CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok();
    }
}