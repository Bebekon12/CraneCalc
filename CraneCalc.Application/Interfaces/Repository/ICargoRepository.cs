using CraneCalc.Application.Features.Cargo.Commands.UpdateCargo;
using CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface ICargoRepository
{
    Task<List<Cargo>> GetCargosPaginatedAsync(
        GetCargosPaginatedQuery filter,
        int pageNumber,
        int pageSize,
        CancellationToken ct);
    
    Task<Cargo> CreateCargoAsync(Cargo cargo, CancellationToken ct);
    Task<Cargo?> UpdateCargoAsync(Guid id, UpdateCargoCommand cargo, CancellationToken ct);
    Task DeleteCargoAsync(Guid id, CancellationToken ct);
    
    Task<Cargo> GetCargoByIdAsync(Guid id, CancellationToken ct);
    Task PutCargoInCartAsync(Guid cargoId, CancellationToken ct);
    Task<string> AddOrUpdateCargoPhotoAsync(Guid cargoId, Stream fileStream, CancellationToken ct);
}