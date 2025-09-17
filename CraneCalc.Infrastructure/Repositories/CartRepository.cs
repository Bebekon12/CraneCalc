using CraneCalc.Application.Contracts.Request;
using CraneCalc.Application.Interfaces;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.EntityFrameworkCore;
using Minio.Exceptions;

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

    public async Task<List<Cart>> GetFilteredCartsAsync(DateTime from, DateTime before, CancellationToken ct)
    {
        var carts = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(c => !c.IsDeleted && c.CreatedDate >= from && c.CreatedDate <= before)
            .Select(c=>c.Map())
            .ToListAsync(ct);
        
        return carts;
    }

    public async Task<Cart?> UpdateCartAsync(Guid id, UpdateCartRequest cart, CancellationToken ct)
    {
        var entity = await context.Carts.FirstOrDefaultAsync(c=>c.Id==id,ct);
        
        if(entity == null)
            return null;
        
        entity.LoadCapacity = cart.LoadCapacity;
        entity.LiftingHeight = cart.LiftingHeight;
        entity.JibOutreach = cart.JibOutreach;
        entity.LiftingSpeed = cart.LiftingSpeed;
        
        await context.SaveChangesAsync(ct);
        return entity.Map();
    }

    public async Task<Cart?> FormCartAsync(Guid cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(cartEntity => cartEntity.CartCargo)
            .FirstOrDefaultAsync(c=>c.Id == cartId, ct);
        
        if(cart == null)
            return null;
        
        if (cart.CreatorId != 1) throw new ArgumentException("Только создатель может формировать заявку");
        
        if (cart.LoadCapacity <= 0 || cart.LiftingHeight <= 0 || 
            cart.JibOutreach <= 0 || cart.LiftingSpeed <= 0)
            throw new ErrorResponseException("Все технические параметры должны быть заполнены");
        
        if (cart.CartCargo.Count == 0)
            throw new ErrorResponseException("Добавьте хотя бы один груз");
        
        cart.FormationDate = DateTime.UtcNow;
        cart.Status = Status.Formed;
        
        await context.SaveChangesAsync(ct);
        
        return cart.Map();
    }

    public async Task<Cart?> ModerateCartAsync(Guid cartId, int userId, bool isApproved, CancellationToken ct)
    {
        var cart = await context.Carts.FirstOrDefaultAsync(c=>c.Id == cartId, ct);
        
        if(cart == null)
            return null;

        if (cart.ModeratorId != null && cart.ModeratorId==userId)
        {
            cart.CompletionDate = DateTime.UtcNow;
            if (isApproved)
            {
                cart.Status = Status.Completed;
                cart.CalculationResult = 11;
            }
            else
            {
                cart.Status = Status.Rejected;
            }
        }
        
        await context.SaveChangesAsync(ct);
        return cart.Map();
    }

    public async Task<string?> DeleteCartAsync(Guid cartId, int userId, CancellationToken ct)
    {
        var cart = await context.Carts.FirstOrDefaultAsync(c=>c.Id == cartId, ct);

        if (cart?.ModeratorId == null || cart.ModeratorId != userId) return null;
        
        cart.IsDeleted = true;
        cart.Status = Status.Deleted;
        await context.SaveChangesAsync(ct);
            
        return "success";

    }
}