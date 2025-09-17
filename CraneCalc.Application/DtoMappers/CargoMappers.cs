using CraneCalc.Application.Contracts.Request;
using CraneCalc.Application.Contracts.Response;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.DtoMappers;

public static class CargoMappers
{
    public static Cargo Map(this CreateCargoRequest model)
    {
        return new Cargo
        {
            Id = Guid.NewGuid(),
            ConcreteGrade = model.ConcreteGrade,
            Type = model.Type,
            Weight = model.Weight,
            Description = model.Description,
            Title = model.Title,
            Volume = model.Volume,
            Dimensions = new Dimensions
            {
                Width = model.Width,
                Height = model.Height,
                Length = model.Length
            }
        };
    }

    public static CargoResponse Map(this Cargo model)
    {
        return new CargoResponse
        {
            Id = model.Id,
            ConcreteGrade = model.ConcreteGrade,
            Type = model.Type,
            Weight = model.Weight,
            Description = model.Description,
            Title = model.Title,
            Volume = model.Volume,
            Height = model.Dimensions.Height,
            Length = model.Dimensions.Length,
            ImageUrl = model.ImageUrl,
            Width = model.Dimensions.Width,
            IsDeleted = model.IsDeleted,
        };
    }
}