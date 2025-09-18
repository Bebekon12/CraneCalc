using CraneCalc.Application.Features.Cargo.Commands.UpdateCargo;
using CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface ICargoRepository
{
    Task<List<CargoModel>> GetCargosPaginatedAsync(
        GetCargosPaginatedQuery filter,
        int pageNumber,
        int pageSize,
        CancellationToken ct);
    
    Task<CargoModel> CreateCargoAsync(CargoModel cargoModel, CancellationToken ct);
    Task<CargoModel?> UpdateCargoAsync(Guid id, UpdateCargoCommand cargo, CancellationToken ct);
    Task DeleteCargoAsync(Guid id, CancellationToken ct);
    
    Task<CargoModel> GetCargoByIdAsync(Guid id, CancellationToken ct);
    Task PutCargoInCartAsync(Guid cargoId, Guid creatorId, bool isModerator, CancellationToken ct);
    Task<string> AddOrUpdateCargoPhotoAsync(Guid cargoId, Stream fileStream, CancellationToken ct);
}