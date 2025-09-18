using CraneCalc.Application.Features.Cart.Commands.UpdateCart;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface ICartRepository
{
    Task<CartModel?> GetCartByIdAsync(Guid cartId, CancellationToken ct);
    Task<CartModel?> GetCartByUserIdAsync(Guid userId, CancellationToken ct);
    Task<List<CartModel>> GetFilteredCartsAsync(DateTime from, DateTime before, CancellationToken ct);
    Task<CartModel?> UpdateCartAsync(Guid id, UpdateCartCommand cart, CancellationToken ct);
    Task<CartModel?> FormCartAsync(Guid cartId, CancellationToken ct);
    Task<CartModel?> ModerateCartAsync(Guid cartId, Guid userId, bool isApproved, CancellationToken ct);
    Task<string?> DeleteCartAsync(Guid cartId, Guid userId, CancellationToken ct);
}