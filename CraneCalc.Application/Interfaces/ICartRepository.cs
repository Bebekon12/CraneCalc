using CraneCalc.Application.Dtos.Request;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByIdAsync(Guid cartId, CancellationToken ct);
    Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken ct);
    Task<List<Cart>> GetFilteredCartsAsync(DateTime from, DateTime before, CancellationToken ct);
    Task<Cart?> UpdateCartAsync(Guid id, UpdateCartRequest cart, CancellationToken ct);
    Task<Cart?> FormCartAsync(Guid cartId, CancellationToken ct);
    Task<Cart?> ModerateCartAsync(Guid cartId, int userId, bool isApproved, CancellationToken ct);
    Task<string?> DeleteCartAsync(Guid cartId, int userId, CancellationToken ct);
}