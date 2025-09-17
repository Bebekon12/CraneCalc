using CraneCalc.Application.Features.Cargo.Commands.AddImageToCargo;
using CraneCalc.Application.Features.Cargo.Commands.CreateCargo;
using CraneCalc.Application.Features.Cargo.Commands.DeleteCargo;
using CraneCalc.Application.Features.Cargo.Commands.PutCargoInCart;
using CraneCalc.Application.Features.Cargo.Commands.UpdateCargo;
using CraneCalc.Application.Features.Cargo.Queries.GetCargo;
using CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/cargo")]
public class CargoController(IMediator mediator) : ControllerBase
{
    [HttpGet("paginated")]
    public async Task<IActionResult> GetCargosPaginated(
        [FromQuery] GetCargosPaginatedQuery query,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(query, ct);
    
        return Ok(result);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCargo([FromQuery] GetCargoQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCargo([FromBody] CreateCargoCommand request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCargo([FromBody] UpdateCargoCommand request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCargo([FromQuery] DeleteCargoCommand request, CancellationToken ct)
    {
        await mediator.Send(request, ct);
        
        return NoContent();
    }

    [HttpPost("put-in-cart")]
    public async Task<IActionResult> PutCargoInCart([FromQuery] PutCargoInCartCommand request, CancellationToken ct)
    {
        await mediator.Send(request, ct);

        return NoContent();
    }

    [HttpPost("add-image")]
    public async Task<IActionResult> AddImageToCargo([FromQuery] Guid cargoId, IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0)
            return BadRequest("Файл не выбран или пуст.");
        
        await using var stream = file.OpenReadStream();
        
        var command = new AddCargoImageCommand(cargoId, stream);
        
        var result = await mediator.Send(command, ct);
        
        return Ok(result);
    }
}