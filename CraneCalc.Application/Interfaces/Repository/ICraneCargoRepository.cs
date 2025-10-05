namespace CraneCalc.Application.Interfaces.Repository;

public interface ICraneCargoRepository
{
    Task<string?> DeleteCargoInCraneOrderAsync(Guid craneOrderId, Guid cargoId, CancellationToken ct);
    Task<string?> UpdateCargoInCraneOrderAsync(Guid cargoId, Guid craneOrderId, string safetyComment, CancellationToken ct);
}