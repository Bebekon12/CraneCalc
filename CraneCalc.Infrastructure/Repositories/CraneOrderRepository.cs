using AutoMapper;
using CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CraneOrderRepository(AppDbContext context, IMapper mapper) : ICraneOrderRepository
{
    public async Task<CraneOrderModel?> GetCraneOrderByIdAsync(Guid craneOrderId, CancellationToken ct)
    {
        var cart = await context.Orders
            .Include(c=>c.CartCargo)
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>r.Status == Status.Draft && !r.IsDeleted)
            .FirstOrDefaultAsync(r=>r.Id == craneOrderId, ct);

        return cart == null 
            ? null 
            : mapper.Map<CraneOrderModel>(cart);
    }

    public async Task<CraneOrderModel?> GetCraneOrderByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var cart = await context.Orders
            .Include(c=>c.CartCargo)
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>r.Status == Status.Draft && !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.CreatorId == userId, ct);

        return cart==null 
            ? null
            : mapper.Map<CraneOrderModel>(cart);
    }

    public async Task<List<CraneOrderModel>> GetFilteredCraneOrderAsync(DateTime from, DateTime before, Status status, CancellationToken ct)
    {
        var utcFrom = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        var utcBefore = DateTime.SpecifyKind(before, DateTimeKind.Utc);
        
        var carts = await context.Orders
            .Include(c=>c.CartCargo)
            .ThenInclude(cc=>cc.Cargo)
            .Where(c => !c.IsDeleted && c.CreatedDate >= utcFrom 
                                     && c.CreatedDate <= utcBefore 
                                     && c.Status==status
                                     && c.Status!=Status.Draft
                                     && c.Status!=Status.Completed)
            .Select(c=>mapper.Map<CraneOrderModel>(c))
            .ToListAsync(ct);
        
        return carts;
    }

    public async Task<CraneOrderModel?> UpdateCraneOrderAsync(Guid id, UpdateCraneOrderCommand craneOrder, CancellationToken ct)
    {
        var entity = await context.Orders.FirstOrDefaultAsync(c=>c.Id==id,ct);
        
        if(entity == null)
            return null;
        
        entity.LoadCapacity = craneOrder.LoadCapacity;
        entity.LiftingHeight = craneOrder.LiftingHeight;
        entity.JibOutreach = craneOrder.JibOutreach;
        entity.LiftingSpeed = craneOrder.LiftingSpeed;
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<CraneOrderModel>(entity);
    }

    public async Task<CraneOrderModel?> FormCraneOrderAsync(Guid craneOrderId, CancellationToken ct)
    {
        var cart = await context.Orders
            .Include(cartEntity => cartEntity.CartCargo)
            .FirstOrDefaultAsync(c=>c.Id == craneOrderId, ct);
        
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
        
        return mapper.Map<CraneOrderModel>(cart);
    }

    public async Task<CraneOrderModel?> ModerateCraneOrderAsync(Guid craneOrderId, Guid userId, bool isApproved, CancellationToken ct)
    {
        var cart = await context.Orders
            .Include(cartEntity => cartEntity.CartCargo)
            .ThenInclude(cartCargoEntity => cartCargoEntity.Cargo)
            .FirstOrDefaultAsync(c=>c.Id == craneOrderId, ct);
        
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
                    .Where(c => c.CartId == craneOrderId)
                    .ToList();

                foreach (var cargo in cargoList)
                {
                    // Используем массу конкретного груза вместо грузоподъемности крана
                    var productivity = 60 * cargo.Cargo.Weight * kV / tC;
                    cargo.CalculationResult = Math.Round((double)productivity!, 2);
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
        return mapper.Map<CraneOrderModel>(cart);
    }

    public async Task<string?> DeleteCraneOrderAsync(Guid craneOrderId, Guid userId, CancellationToken ct)
    {
        var cart = await context.Orders.FirstOrDefaultAsync(c=>c.Id == craneOrderId, ct);

        if (cart?.CreatorId != userId) 
            return null;
        
        cart.IsDeleted = true;
        cart.Status = Status.Deleted;
        await context.SaveChangesAsync(ct);
            
        return "success";
    }
}