using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;

namespace CraneCalc.Infrastructure.EntityMappers;

public static class CartDomainEntityMapper
{
    public static Cart Map(this CartEntity model)
    {
        return new Cart
        {
            Id = model.Id,
            CompletionDate = model.CompletionDate,
            CalculationResult = model.CalculationResult,
            CreatedDate = model.CreatedDate,
            CreatorId = model.CreatorId,
            FormationDate = model.FormationDate,
            JibOutreach = model.JibOutreach,
            IsDeleted = model.IsDeleted,
            LiftingSpeed = model.LiftingSpeed,
            LiftingHeight = model.LiftingHeight,
            LoadCapacity = model.LoadCapacity,
            CartCargo = model.CartCargo
                .Where(cc => !cc.IsDeleted)
                .Select(c=>new CartCargo
            {
                Id = c.Id,
                CartId = c.CartId,
                CargoId = c.CargoId,
                IsDeleted = c.IsDeleted,
                SafetyComment = c.SafetyComment,
                CalculationResult = c.CalculationResult,
                Cargo = c.Cargo.Map(),
            }).ToList(),
        };
    }
    
    public static CartEntity Map(this Cart model)
    {
        return new CartEntity
        {
            Id = model.Id,
            CompletionDate = model.CompletionDate,
            CalculationResult = model.CalculationResult,
            CreatedDate = model.CreatedDate,
            CreatorId = model.CreatorId,
            FormationDate = model.FormationDate,
            JibOutreach = model.JibOutreach,
            IsDeleted = model.IsDeleted,
            LiftingSpeed = model.LiftingSpeed,
            LiftingHeight = model.LiftingHeight,
            LoadCapacity = model.LoadCapacity,
            CartCargo = model.CartCargo
                .Where(cc => !cc.IsDeleted)
                .Select(c=>new CartCargoEntity
            {
                Id = c.Id,
                CartId = c.CartId,
                CargoId = c.CargoId,
                IsDeleted = c.IsDeleted,
                SafetyComment = c.SafetyComment,
                CalculationResult = c.CalculationResult,
                Cargo = c.Cargo.Map(),
            }).ToList(),
        };
    }
}