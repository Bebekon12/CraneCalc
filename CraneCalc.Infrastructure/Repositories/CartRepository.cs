using CraneCalc.Application.Interfaces;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CartRepository(AppDbContext context) : ICartRepository
{
    public async Task<Cart?> GetCartByIdAsync(Guid cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>!r.IsDeleted)
            .FirstOrDefaultAsync(r=>r.Id == cartId, ct);

        return cart?.Map();
    }

    public async Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>!r.IsDeleted)
            .FirstOrDefaultAsync(r => r.CreatorId == userId, ct);

        return cart?.Map();
    }

    public async Task<Cart> CreateCartAsync(Cart cart, CancellationToken ct)
    {
        var requestEntity = cart.Map();
        
        await context.Carts.AddAsync(requestEntity, ct);
        await context.SaveChangesAsync(ct);
        
        return cart;
    }

    public async Task RemoveCargoInCartAsync(Guid cartId, Guid cargoId, CancellationToken ct)
    {
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
                UPDATE "CartCargos" 
                SET "IsDeleted" = true
                WHERE "CartId" = {cartId} 
                AND "CargoId" = {cargoId} 
                AND "IsDeleted" = false
             """, 
            ct);
    }
    
    public async Task RemoveCartAsync(Guid cartId, CancellationToken ct)
    {
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
                UPDATE "Carts"
                SET "IsDeleted" = true
                SET "Status" = 1
                WHERE "Id" = {cartId} 
                AND "IsDeleted" = false
             """, 
            ct);
    }
}