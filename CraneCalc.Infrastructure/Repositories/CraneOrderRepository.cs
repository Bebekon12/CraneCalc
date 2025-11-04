using AutoMapper;
using CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;
using CraneCalc.Application.Features.DTOs.Request;
using CraneCalc.Application.Features.DTOs.Response;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CraneOrderRepository(AppDbContext context, IMapper mapper, ICraneCalculationService calculationService) : ICraneOrderRepository
{
    public async Task<CraneOrderModel?> GetCraneOrderByIdAsync(Guid craneOrderId, CancellationToken ct)
    {
        var craneOrder = await context.Orders
            .Include(c=>c.CraneCargo)
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>r.Status == Status.Draft && !r.IsDeleted)
            .FirstOrDefaultAsync(r=>r.Id == craneOrderId, ct);

        return craneOrder == null 
            ? null 
            : mapper.Map<CraneOrderModel>(craneOrder);
    }

    public async Task<CraneOrderModel?> GetCraneOrderByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var craneOrder = await context.Orders
            .Include(c=>c.CraneCargo)
            .ThenInclude(cc=>cc.Cargo)
            .Where(r=>r.Status == Status.Draft && !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.CreatorId == userId, ct);

        return craneOrder==null 
            ? null
            : mapper.Map<CraneOrderModel>(craneOrder);
    }

    public async Task<List<CraneOrderModel>> GetFilteredCraneOrderAsync(DateTime from, DateTime before, Status status, CancellationToken ct)
    {
        var utcFrom = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        var utcBefore = DateTime.SpecifyKind(before, DateTimeKind.Utc);
        
        var craneOrders = await context.Orders
            .Include(c=>c.CraneCargo)
            .ThenInclude(cc=>cc.Cargo)
            .Where(c => !c.IsDeleted && c.CreatedDate >= utcFrom 
                                     && c.CreatedDate <= utcBefore 
                                     && c.Status==status
                                     && c.Status!=Status.Draft
                                     && c.Status!=Status.Completed)
            .Select(c=>mapper.Map<CraneOrderModel>(c))
            .ToListAsync(ct);
        
        return craneOrders;
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
        var craneOrder = await context.Orders
            .Include(cartEntity => cartEntity.CraneCargo)
            .FirstOrDefaultAsync(c=>c.Id == craneOrderId, ct);
        
        if(craneOrder == null)
            return null;
        
        if (craneOrder.ModeratorId==null) 
            throw new EntityException("Только создатель может формировать заявку");
        
        if (craneOrder.LoadCapacity <= 0 || craneOrder.LiftingHeight <= 0 || 
            craneOrder.JibOutreach <= 0 || craneOrder.LiftingSpeed <= 0)
            throw new EntityException("Все технические параметры должны быть заполнены");
        
        if (craneOrder.CraneCargo.Count == 0)
            throw new EntityException("Добавьте хотя бы один груз");
        
        craneOrder.FormationDate = DateTime.UtcNow;
        craneOrder.Status = Status.Formed;
        
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<CraneOrderModel>(craneOrder);
    }

    public async Task<CraneOrderModel?> ModerateCraneOrderAsync(Guid craneOrderId, Guid userId, bool isApproved, CancellationToken ct)
    {
        var craneOrder = await context.Orders
            .Include(cartEntity => cartEntity.CraneCargo)
            .ThenInclude(cartCargoEntity => cartCargoEntity.Cargo)
            .FirstOrDefaultAsync(c => c.Id == craneOrderId, ct);
        
        if(craneOrder == null)
            return null;

        if (craneOrder.Status != Status.Formed)
            throw new EntityException("Заявка не сформирована");

        if (craneOrder.ModeratorId != null && craneOrder.ModeratorId == userId)
        {
            craneOrder.CompletionDate = DateTime.UtcNow;
            if (isApproved)
            {
                craneOrder.Status = Status.Completed;
                
                // Инициируем асинхронный расчет (не ждем результат)
                _ = InitiateCalculationAsync(craneOrderId, craneOrder, ct);
            }
            else
            {
                craneOrder.Status = Status.Rejected;
            }
        }
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<CraneOrderModel>(craneOrder);
    }

    private async Task InitiateCalculationAsync(Guid craneOrderId, CraneOrderEntity craneOrder, CancellationToken ct)
    {
        try
        {
            // Проверяем, что все необходимые параметры заполнены
            if (craneOrder.LiftingHeight.HasValue && 
                craneOrder.LiftingSpeed.HasValue && 
                craneOrder.JibOutreach.HasValue)
            {
                var calculationRequest = new CalculationRequest
                {
                    CraneOrderId = craneOrderId.ToString(),
                    LiftingHeight = craneOrder.LiftingHeight,
                    LiftingSpeed = craneOrder.LiftingSpeed,
                    JibOutreach = craneOrder.JibOutreach,
                    Cargos = craneOrder.CraneCargo.Select(cc => new CargoRequest
                    {
                        Id = cc.CargoId.ToString(),
                        Weight = cc.Cargo.Weight
                    }).ToList()
                };

                // Отправляем запрос на расчет (не ждем результат)
                // FastAPI сам отправит результаты через метод update-calculation
                await calculationService.CalculateCraneProductivityAsync(calculationRequest, ct);
            }
        }
        catch (Exception ex)
        {
            // Логируем ошибку, но не прерываем выполнение
            Console.WriteLine($"Error initiating calculation: {ex.Message}");
        }
    }

    private async Task UpdateCalculationResultsAsync(Guid craneOrderId, CalculationResponse result, CancellationToken ct)
    {
        var craneOrder = await context.Orders
            .Include(o => o.CraneCargo)
            .FirstOrDefaultAsync(o => o.Id == craneOrderId, ct);

        if (craneOrder != null && result.IsSuccess)
        {
            // Обновляем результаты для каждого груза
            foreach (var cargoResult in result.CargoResults)
            {
                var craneCargo = craneOrder.CraneCargo.FirstOrDefault(cc => cc.CargoId.ToString() == cargoResult.CargoId);
                if (craneCargo != null)
                {
                    craneCargo.CalculationResult = cargoResult.CalculationResult;
                }
            }

            // Обновляем общий результат
            craneOrder.CalculationResult = result.TotalCalculationResult;

            await context.SaveChangesAsync(ct);
        }
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

    public async Task<CraneOrderModel?> GetByIdWithCargoAsync(Guid id, CancellationToken ct)
    {
        return mapper.Map<CraneOrderModel>(await context.Orders
            .Include(o => o.CraneCargo)
            .FirstOrDefaultAsync(o => o.Id == id, ct));
    }

    public async Task UpdateCalculationResultsAsync(Guid craneOrderId, double? totalResult, List<CargoCalculationResult> cargoResults, CancellationToken ct)
    {
        // Получаем entity, а не модель
        var craneOrder = await context.Orders
            .Include(o => o.CraneCargo)
            .FirstOrDefaultAsync(o => o.Id == craneOrderId, ct);
    
        if (craneOrder == null) return;

        // Обновляем общий результат
        craneOrder.CalculationResult = totalResult;

        // Обновляем результаты для каждого груза
        foreach (var cargoResult in cargoResults)
        {
            var craneCargo = craneOrder.CraneCargo.FirstOrDefault(cc => cc.CargoId.ToString() == cargoResult.CargoId);
            if (craneCargo != null)
            {
                craneCargo.CalculationResult = cargoResult.CalculationResult;
            }
        }

        // Сохраняем изменения
        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct) > 0;
    }
}