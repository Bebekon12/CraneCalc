using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByIdAsync(Guid cartId, CancellationToken ct);
    Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken ct);
    Task<List<Cart>> GetFilteredCartsAsync(DateTime from, DateTime before, CancellationToken ct);
    Task RemoveCargoInCartAsync(Guid cartId, Guid cargoId, CancellationToken ct);
    Task RemoveCartAsync(Guid cartId, CancellationToken ct);
}