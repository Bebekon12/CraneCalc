using AutoMapper;
using CraneCalc.Application.Features.Cart.Commands.UpdateCart;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CartRepository(AppDbContext context, IMapper mapper) : ICartRepository
{
    public async Task<CartModel?> GetCartByIdAsync(Guid cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>r.Status == Status.Draft && !r.IsDeleted)
            .FirstOrDefaultAsync(r=>r.Id == cartId, ct);

        return cart == null 
            ? null 
            : mapper.Map<CartModel>(cart);
    }

    public async Task<CartModel?> GetCartByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>r.Status == Status.Draft && !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.CreatorId == userId, ct);

        return cart==null 
            ? null
            : mapper.Map<CartModel>(cart);
    }

    public async Task<List<CartModel>> GetFilteredCartsAsync(DateTime from, DateTime before, Status status, CancellationToken ct)
    {
        var utcFrom = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        var utcBefore = DateTime.SpecifyKind(before, DateTimeKind.Utc);
        
        var carts = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(c => !c.IsDeleted && c.CreatedDate >= utcFrom 
                                     && c.CreatedDate <= utcBefore 
                                     && c.Status==status
                                     && c.Status!=Status.Draft
                                     && c.Status!=Status.Completed)
            .Select(c=>mapper.Map<CartModel>(c))
            .ToListAsync(ct);
        
        return carts;
    }

    public async Task<CartModel?> UpdateCartAsync(Guid id, UpdateCartCommand cart, CancellationToken ct)
    {
        var entity = await context.Carts.FirstOrDefaultAsync(c=>c.Id==id,ct);
        
        if(entity == null)
            return null;
        
        entity.LoadCapacity = cart.LoadCapacity;
        entity.LiftingHeight = cart.LiftingHeight;
        entity.JibOutreach = cart.JibOutreach;
        entity.LiftingSpeed = cart.LiftingSpeed;
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<CartModel>(entity);
    }

    public async Task<CartModel?> FormCartAsync(Guid cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(cartEntity => cartEntity.CartCargo)
            .FirstOrDefaultAsync(c=>c.Id == cartId, ct);
        
        if(cart == null)
            return null;
        
        if (cart.ModeratorId==null) 
            throw new EntityException("Только создатель может формировать заявку");
        
        if (cart.LoadCapacity <= 0 || cart.LiftingHeight <= 0 || 
            cart.JibOutreach <= 0 || cart.LiftingSpeed <= 0)
            throw new EntityException("Все технические параметры должны быть заполнены");
        
        if (cart.CartCargo.Count == 0)
            throw new EntityException("Добавьте хотя бы один груз");
        
        cart.FormationDate = DateTime.UtcNow;
        cart.Status = Status.Formed;
        
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<CartModel>(cart);
    }

    public async Task<CartModel?> ModerateCartAsync(Guid cartId, Guid userId, bool isApproved, CancellationToken ct)
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
                
                // Константы
                const double kV = 0.85;  // Коэффициент использования крана
                const double tR = 0.5;   // Время ручных операций (мин)
                const double vT = 30;    // Скорость тележки (м/мин)
                const double vD = 30;    // Скорость движения крана (м/мин)
                const double n1 = 1;     // Число поворотов стрелы
                const double n = 2;       // Максимальное число поворотов
                const double kSov = 1.0; // Коэффициент совмещения операций

                // Параметры из заявки
                var q = cart.LoadCapacity;        // Грузоподъемность (т)
                var h = cart.LiftingHeight;       // Высота подъема (м)
                var vP = cart.LiftingSpeed;      // Скорость подъема (м/мин)
                var lT = cart.JibOutreach;       // Вылет стрелы (м)
                var lD = cart.JibOutreach * 0.5; // Предполагаемое расстояние движения крана

                // Расчет времени цикла
                var tM = 2.5 * (h / vP) + 2 * (lT / vT + lD / vD + n1 / n) * kSov;
                var tC = tM + tR;

                // Расчет производительности (т/ч)
                var productivity = 60 * q * kV / tC;

                cart.CalculationResult = Math.Round(productivity, 2);
            }
            else
            {
                cart.Status = Status.Rejected;
            }
        }
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<CartModel>(cart);
    }

    public async Task<string?> DeleteCartAsync(Guid cartId, Guid userId, CancellationToken ct)
    {
        var cart = await context.Carts.FirstOrDefaultAsync(c=>c.Id == cartId, ct);

        if (cart?.CreatorId != userId) 
            return null;
        
        cart.IsDeleted = true;
        cart.Status = Status.Deleted;
        await context.SaveChangesAsync(ct);
            
        return "success";
    }
}