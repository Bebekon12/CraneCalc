using AutoMapper;
using CraneCalc.Application.Contracts.Request;
using CraneCalc.Application.Contracts.Response;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/cargo")]
public class CargoController(ICargoRepository cargoRepository, IMapper mapper) : ControllerBase
{
    [HttpGet("paginated")]
    public async Task<IActionResult> GetCargosPaginated(
        [FromQuery] CargoFilter filter,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var cargos = await cargoRepository.GetCargosPaginatedAsync(
            filter, 
            pageNumber, 
            pageSize, 
            ct);
    
        return Ok(cargos);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCargo(Guid cargoId, CancellationToken ct)
    {
        var cargo = await cargoRepository.GetCargoByIdAsync(cargoId, ct);
        
        return Ok(cargo);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCargo([FromBody] CreateCargoRequest request, CancellationToken ct)
    {
        var createdCargo = await cargoRepository.CreateCargoAsync(mapper.Map<Cargo>(request), ct);
        
        return Ok(mapper.Map<CargoResponse>(createdCargo));
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCargo([FromQuery] Guid id, [FromBody] UpdateCargoRequest request, CancellationToken ct)
    {
        var updatedCargo = await cargoRepository.UpdateCargoAsync(id, request, ct);
        
        if(updatedCargo == null)
            return NotFound();
        
        return Ok(mapper.Map<CargoResponse>(updatedCargo));
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCargo(Guid cargoId, CancellationToken ct)
    {
        await cargoRepository.DeleteCargoAsync(cargoId, ct);
        
        return Ok();
    }

    [HttpPost("put-in-cart")]
    public async Task<IActionResult> PutCargoInCart(Guid cargoId, CancellationToken ct)
    {
        await cargoRepository.PutCargoInCartAsync(cargoId, ct);

        return Ok();
    }

    [HttpPost("add-image")]
    public async Task<IActionResult> AddImageToCargo(Guid cargoId, IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var fileName = await cargoRepository.AddOrUpdateCargoPhotoAsync(cargoId, stream, ct);
        
        return Ok(new
        {
            FileName = fileName
        });
    }
}