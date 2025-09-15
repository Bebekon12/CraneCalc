using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces;

public interface ICargoRepository
{
    Task<List<Cargo>> GetCargosPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<List<Cargo>> GetCargosAsync(CancellationToken ct);
    Task<List<Cargo>> GetCargosSearchAsync(string search, CancellationToken ct);
    Task<Cargo> GetCargoByIdAsync(Guid id, CancellationToken ct);
    Task PutCargoInCartAsync(Guid cargoId, Guid cartId, CancellationToken ct);
}