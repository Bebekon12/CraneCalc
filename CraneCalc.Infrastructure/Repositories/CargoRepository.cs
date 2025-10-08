using AutoMapper;
using CraneCalc.Application.Features.Cargo.Commands.UpdateCargo;
using CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CargoRepository(
        AppDbContext context,
        IFileStorageService storageService,
        IMapper mapper) : ICargoRepository
{
    public async Task<List<CargoModel>> GetCargosPaginatedAsync(
        GetCargosPaginatedQuery filter,
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        var query = context.Cargos.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(c => c.Title.ToLower().Contains(filter.Title.ToLower()));

        if (!string.IsNullOrEmpty(filter.Type))
            query = query.Where(c => c.Type.ToLower() == filter.Type.ToLower());

        if (filter.MinWeight.HasValue)
            query = query.Where(c => c.Weight >= filter.MinWeight.Value);

        if (filter.MaxWeight.HasValue)
            query = query.Where(c => c.Weight <= filter.MaxWeight.Value);
        
        var cargosPaginated = await query
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => mapper.Map<CargoModel>(c))
            .ToListAsync(ct);
    
        return cargosPaginated;
    }

    public async Task<CargoModel> CreateCargoAsync(CargoModel cargoModel, CancellationToken ct)
    {
        var cargoEntity = mapper.Map<CargoEntity>(cargoModel);
        
        await context.Cargos.AddAsync(cargoEntity, ct);
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<CargoModel>(cargoEntity);
    }

    public async Task<CargoModel?> UpdateCargoAsync(Guid id, UpdateCargoCommand cargo, CancellationToken ct)
    {
        var cargoEntity = await context.Cargos
            .FirstOrDefaultAsync(c=>c.Id==id,ct);

        if (cargoEntity == null)
            return null;
        
        if(cargo.Type!=string.Empty && cargo.Type!=cargoEntity.Type)
            cargoEntity.Type = cargo.Type;
        
        if(cargo.Title!=string.Empty && cargo.Title!=cargoEntity.Title)
            cargoEntity.Title = cargo.Title;
        
        if(cargo.Description!=string.Empty && cargo.Description!=cargoEntity.Description)
            cargoEntity.Description = cargo.Description;
        
        if(cargo.Height!=0 && Math.Abs(cargo.Height - cargoEntity.Height) > 0)
            cargoEntity.Height = cargo.Height;
        
        if(cargo.Weight!=0 && Math.Abs(cargo.Weight - cargoEntity.Weight) > 0)
            cargoEntity.Weight = cargo.Weight;
        
        if(cargo.Length!=0 && Math.Abs(cargo.Length - cargoEntity.Length) > 0)
            cargoEntity.Length = cargo.Length;
        
        if(cargo.Width!=0 && Math.Abs(cargo.Width - cargoEntity.Width) > 0)
            cargoEntity.Width = cargo.Width;
        
        if(cargo.ConcreteGrade!=string.Empty && cargo.ConcreteGrade!=cargoEntity.ConcreteGrade)
            cargoEntity.ConcreteGrade = cargo.ConcreteGrade;
        
        if(cargo.Volume!=0 && Math.Abs(cargo.Volume - cargoEntity.Volume) > 0)
            cargoEntity.Volume = cargo.Volume;
        
        await context.SaveChangesAsync(ct);
        return mapper.Map<CargoModel>(cargoEntity);
    }

    public async Task DeleteCargoAsync(Guid id, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        
        if (cargo == null)
            throw new EntityException($"Cargo with id:{id} not found");
        
        if (!string.IsNullOrEmpty(cargo.ImageUrl))
        {
            await storageService.DeletePhotoAsync(cargo.ImageUrl, ct);
        }
        
        context.Cargos.Remove(cargo);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task<CargoModel> GetCargoByIdAsync(Guid id, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        
        if(cargo == null)
            throw new EntityException("Cargo not found");
        
        return mapper.Map<CargoModel>(cargo);
    }

    public async Task PutCargoInCraneOrderAsync(Guid cargoId, Guid creatorId, bool isModerator, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == cargoId, ct);
    
        if(cargo == null)
            throw new NullReferenceException("Cargo not found");

        var cart = await context.Orders
            .Include(c => c.CraneCargo)
            .FirstOrDefaultAsync(c => c.CreatorId == creatorId && !c.IsDeleted && c.Status == Status.Draft, ct);

        if(cart?.CraneCargo.Any(cc => cc.CargoId == cargoId) == true)
            throw new EntityException($"Cargo already purchased");

        CraneOrderEntity targetCart;
    
        if(cart != null)
        {
            targetCart = cart;
        }
        else
        {
            targetCart = new CraneOrderEntity
            {
                Id = Guid.NewGuid(),
                ModeratorId = isModerator ? creatorId : null,
                CreatorId = creatorId,
                Status = Status.Draft
            };
            await context.Orders.AddAsync(targetCart, ct);
        }
    
        var cartCargo = new CraneCargoEntity
        {
            CraneOrderId = targetCart.Id,
            CargoId = cargoId,
            CalculationResult = 0,
            SafetyComment = string.Empty,
        };
    
        context.CraneCargos.Add(cartCargo);
        await context.SaveChangesAsync(ct);
    }

    public async Task<string> AddOrUpdateCargoPhotoAsync(Guid cargoId, Stream fileStream, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == cargoId, ct);
        
        if (cargo == null)
            throw new EntityException($"Cargo with id:{cargoId} not found");

        var newFileName = await storageService.GenerateFileName();
        
        await storageService.UploadPhotoAsync(fileStream, newFileName, ct);
        
        if (!string.IsNullOrEmpty(cargo.ImageUrl))
        {
            await storageService.DeletePhotoAsync(cargo.ImageUrl, ct);
        }
        
        cargo.ImageUrl = newFileName;
        await context.SaveChangesAsync(ct);
        
        return newFileName;
    }
}