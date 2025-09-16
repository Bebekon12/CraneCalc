using CraneCalc.Application.Dtos.Request;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces;

public interface ICargoRepository
{
    Task<List<Cargo>> GetCargosPaginatedAsync(
        CargoFilter filter,
        int pageNumber,
        int pageSize,
        CancellationToken ct);
    
    Task<Cargo> CreateCargoAsync(Cargo cargo, CancellationToken ct);
    Task<Cargo?> UpdateCargoAsync(Guid id, UpdateCargoRequest cargo, CancellationToken ct);
    Task DeleteCargoAsync(Guid id, CancellationToken ct);
    
    Task<Cargo> GetCargoByIdAsync(Guid id, CancellationToken ct);
    Task PutCargoInCartAsync(Guid cargoId, CancellationToken ct);
    Task<string> AddOrUpdateCargoPhotoAsync(Guid cargoId, Stream fileStream, CancellationToken ct);
}