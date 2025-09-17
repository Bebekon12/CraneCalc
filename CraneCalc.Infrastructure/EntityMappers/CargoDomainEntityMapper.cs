using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;

namespace CraneCalc.Infrastructure.EntityMappers;

public static class CargoDomainEntityMapper
{
    public static Cargo Map(this CargoEntity model)
    {
        return new Cargo
        {
            Id = model.Id,
            ConcreteGrade = model.ConcreteGrade,
            Description = model.Description,
            Dimensions = new Dimensions
            {
                Width = model.Width,
                Height = model.Height,
                Length = model.Length,
            },
            ImageUrl = model.ImageUrl,
            IsDeleted = model.IsDeleted,
            Title = model.Title,
            Type = model.Type,
            Volume = model.Volume,
            Weight = model.Weight,
        };
    }
    
    public static CargoEntity Map(this Cargo model)
    {
        return new CargoEntity
        {
            Id = model.Id,
            ConcreteGrade = model.ConcreteGrade,
            Description = model.Description,
            Width = model.Dimensions.Width,
            Height = model.Dimensions.Height,
            Length = model.Dimensions.Length,
            ImageUrl = model.ImageUrl,
            IsDeleted = model.IsDeleted,
            Title = model.Title,
            Type = model.Type,
            Volume = model.Volume,
            Weight = model.Weight,
        };
    }
}