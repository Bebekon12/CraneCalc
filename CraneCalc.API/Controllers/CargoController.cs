using CraneCalc.Application.Dtos.Request;
using CraneCalc.Application.Interfaces;
using CraneCalc.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargoController(ICargoRepository cargoRepository) : ControllerBase
{
    [HttpGet("cargos-paginated")]
    public async Task<IActionResult> GetCargosPaginated(int pageNumber, int pageSize, CancellationToken ct)
    {
        var cargos = await cargoRepository.
            GetCargosPaginatedAsync(pageNumber, pageSize, ct);
        
        return Ok(cargos);
    }

    [HttpGet("cargo")]
    public async Task<IActionResult> GetCargo(Guid cargoId, CancellationToken ct)
    {
        var cargo = await cargoRepository.GetCargoByIdAsync(cargoId, ct);
        
        return Ok(cargo);
    }
    
}