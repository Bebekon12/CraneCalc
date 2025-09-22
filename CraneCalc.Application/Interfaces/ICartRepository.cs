using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByIdAsync(Guid cartId, CancellationToken ct);
    Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken ct);
    
    Task<Cart> CreateCartAsync(Cart cart, CancellationToken cancellationToken);
    
    Task RemoveCartAsync(Guid cartId, CancellationToken ct);
}