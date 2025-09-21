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
    public async Task<Cart?> GetCartByIdAsync(Guid cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>!r.IsDeleted)
            .FirstOrDefaultAsync(r=>r.Id == cartId, ct);

        return cart == null 
            ? null 
            : mapper.Map<Cart>(cart);
    }

    public async Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>!r.IsDeleted)
            .FirstOrDefaultAsync(r => r.CreatorId == userId, ct);

        return cart==null 
            ? null
            : mapper.Map<Cart>(cart);
    }
    

    public async Task<List<Cart>> GetFilteredCartsAsync(DateTime from, DateTime before, Status status, CancellationToken ct)
    {
        var carts = await context.Carts
            .Include(c=>c.CartCargo.Where(cc => !cc.IsDeleted))
            .ThenInclude(cc=>cc.Cargo)
            .Where(c => !c.IsDeleted && c.CreatedDate >= from && c.CreatedDate <= before && c.Status == status)
            .Select(c=>mapper.Map<Cart>(c))
            .ToListAsync(ct);
        
        return carts;
    }

    public async Task<Cart?> UpdateCartAsync(Guid id, UpdateCartCommand cart, CancellationToken ct)
    {
        var entity = await context.Carts.FirstOrDefaultAsync(c=>c.Id==id,ct);
        
        if(entity == null)
            return null;
        
        entity.LoadCapacity = cart.LoadCapacity;
        entity.LiftingHeight = cart.LiftingHeight;
        entity.JibOutreach = cart.JibOutreach;
        entity.LiftingSpeed = cart.LiftingSpeed;
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<Cart>(entity);
    }

    public async Task<Cart?> FormCartAsync(Guid cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(cartEntity => cartEntity.CartCargo)
            .FirstOrDefaultAsync(c=>c.Id == cartId, ct);
        
        if(cart == null)
            return null;
        
        if (cart.CreatorId != 1) throw new EntityException("Только создатель может формировать заявку");
        
        if (cart.LoadCapacity <= 0 || cart.LiftingHeight <= 0 || 
            cart.JibOutreach <= 0 || cart.LiftingSpeed <= 0)
            throw new EntityException("Все технические параметры должны быть заполнены");
        
        if (cart.CartCargo.Count == 0)
            throw new EntityException("Добавьте хотя бы один груз");
        
        cart.FormationDate = DateTime.UtcNow;
        cart.Status = Status.Formed;
        
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<Cart>(cart);
    }

    public async Task<Cart?> ModerateCartAsync(Guid cartId, int userId, bool isApproved, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(cartEntity => cartEntity.CartCargo)
            .ThenInclude(cartCargoEntity => cartCargoEntity.Cargo)
            .FirstOrDefaultAsync(c=>c.Id == cartId, ct);
        
        if(cart == null)
            return null;

        if (cart.ModeratorId != null && cart.ModeratorId == userId)
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
                const double n = 2;      // Максимальное число поворотов
                const double kSov = 1.0; // Коэффициент совмещения операций

                // Параметры из заявки
                var h = cart.LiftingHeight;   // Высота подъема (м)
                var vP = cart.LiftingSpeed;  // Скорость подъема (м/мин)
                var lT = cart.JibOutreach;   // Вылет стрелы (м)
                var lD = cart.JibOutreach * 0.5; // Предполагаемое расстояние движения крана

                // Расчет времени цикла (общий для всех грузов)
                var tM = 2.5 * (h / vP) + 2 * (lT / vT + lD / vD + n1 / n) * kSov;
                var tC = tM + tR;

                // Для каждого груза рассчитываем производительность индивидуально
                var cargoList = cart.CartCargo
                    .Where(c => c.CartId == cartId && !c.IsDeleted)
                    .ToList();

                foreach (var cargo in cargoList)
                {
                    // Используем массу конкретного груза вместо грузоподъемности крана
                    var productivity = 60 * cargo.Cargo.Weight * kV / tC;
                    cargo.CalculationResult = Math.Round(productivity, 2);
                }

                // Общая производительность для всей корзины
                cart.CalculationResult = cargoList.Sum(c => c.CalculationResult);
            }
            else
            {
                cart.Status = Status.Rejected;
            }
        }
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<Cart>(cart);
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