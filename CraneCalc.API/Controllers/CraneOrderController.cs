using CraneCalc.Application.Features.CraneOrder.Commands.DeleteCraneOrder;
using CraneCalc.Application.Features.CraneOrder.Commands.FormCraneOrder;
using CraneCalc.Application.Features.CraneOrder.Commands.ModerateCraneOrder;
using CraneCalc.Application.Features.CraneOrder.Commands.UpdateCalculationResult;
using CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;
using CraneCalc.Application.Features.CraneOrder.Queries.GetCraneOrder;
using CraneCalc.Application.Features.CraneOrder.Queries.GetCraneOrderInfo;
using CraneCalc.Application.Features.CraneOrder.Queries.GetFilteredCraneOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/crane-order")]
public class CraneOrderController(IMediator mediator) : ControllerBase
{
    [HttpGet("info")]
    public async Task<IActionResult> GetCraneOrderInformation(CancellationToken ct)
    {
        var query = new GetCraneOrderInformationQuery();
        var result = await mediator.Send(query, ct);
        
        if(result==null)
            return NotFound();
        
        return Ok(result);
    }

    [Authorize]
    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredCraneOrder([FromQuery] GetFilteredCraneOrderQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        return Ok(result);
    }

    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> GetCraneOrder([FromQuery] GetCraneOrderQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateCraneOrder(
        [FromBody] UpdateCraneOrderCommand request,
        CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }

    [Authorize]
    [HttpPut("form")]
    public async Task<IActionResult> FormCraneOrder([FromQuery] FormCraneOrderCommand query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("moderate")]
    public async Task<IActionResult> ModerateCraneOrder([FromQuery] ModerateCraneOrderCommand query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        if(result == null)
            return NotFound();
        
        return Ok(result);
    }
    
    [HttpPost("update-calculation")]
    public async Task<IActionResult> UpdateCalculationResult([FromBody] UpdateCalculationResultCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
    
        if (!result)
            return Unauthorized("Invalid auth token");
        
        
        return Ok(new { success = true });
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCraneOrder([FromQuery] DeleteCraneOrderCommand query, CancellationToken ct)
    {
        await mediator.Send(query, ct);

        return NoContent();
    }
}