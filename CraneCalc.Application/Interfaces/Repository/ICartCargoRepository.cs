namespace CraneCalc.Application.Interfaces.Repository;

public interface ICartCargoRepository
{
    Task<string?> DeleteCargoInCartAsync(Guid cartId, Guid cargoId, CancellationToken ct);
    Task<string?> UpdateCargoInCartAsync(Guid cargoId, Guid cartId, string safetyComment, CancellationToken ct);
}