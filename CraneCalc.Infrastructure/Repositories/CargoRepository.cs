using CraneCalc.Application.Contracts.Request;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CargoRepository(
        AppDbContext context,
        IFileStorageService storageService) : ICargoRepository
{
    public async Task<List<Cargo>> GetCargosPaginatedAsync(
        CargoFilter filter,
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
            .Select(c => c.Map())
            .ToListAsync(ct);
    
        return cargosPaginated;
    }

    public async Task<Cargo> CreateCargoAsync(Cargo cargo, CancellationToken ct)
    {
        var cargoEntity = cargo.Map();
        
        await context.Cargos.AddAsync(cargoEntity, ct);
        await context.SaveChangesAsync(ct);
        
        return cargo;
    }

    public async Task<Cargo?> UpdateCargoAsync(Guid id, UpdateCargoRequest cargo, CancellationToken ct)
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
        return cargoEntity.Map();
    }

    public async Task DeleteCargoAsync(Guid id, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        
        if (cargo == null)
            throw new Exception($"Cargo with id:{id} not found");
        
        if (!string.IsNullOrEmpty(cargo.ImageUrl))
        {
            await storageService.DeletePhotoAsync(cargo.ImageUrl, ct);
        }
        
        context.Cargos.Remove(cargo);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task<Cargo> GetCargoByIdAsync(Guid id, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        
        if(cargo == null)
            throw new NullReferenceException("Cargo");
        
        return cargo.Map();
    }

    public async Task PutCargoInCartAsync(Guid cargoId, CancellationToken ct)
    {
        var cargo = await context.Cargos.FirstOrDefaultAsync(c => c.Id == cargoId, ct);
        
        if(cargo == null)
            throw new NullReferenceException("Cargo not found");

        var cart = await context.Carts
            .Include(c=>c.CartCargo.Where(i=>!i.IsDeleted))
            .FirstOrDefaultAsync(c => c.CreatorId == 1, ct);

        if(cart != null)
        {
            if(cart.CartCargo.Where(cc=>cc.CargoId == cargoId).ToList().Count > 0)
                throw new Exception($"Cargo already purchased");
            
            cart.CartCargo.Add(new CartCargoEntity
            {
                CartId = cart.Id,
                CargoId = cargoId,
                Cargo = cargo,
                Cart = cart,
                CalculationResult = 0,
                SafetyComment = string.Empty,
            });
            
            await context.SaveChangesAsync(ct);
            
            return;
        }

        var newCart = new CartEntity
        {
            Id = Guid.NewGuid(),
            ModeratorId = 1,
            CreatorId = 1
        };
        
        newCart.CartCargo.Add(new CartCargoEntity
        {
            CartId = newCart.Id,
            CargoId = cargoId,
            Cargo = cargo,
            Cart = newCart,
            CalculationResult = 0,
            SafetyComment = string.Empty,
        });
        
        await context.Carts.AddAsync(newCart, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<string> AddOrUpdateCargoPhotoAsync(Guid cargoId, Stream fileStream, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == cargoId, ct);
        
        if (cargo == null)
            throw new Exception($"Cargo with id:{cargoId} not found");

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