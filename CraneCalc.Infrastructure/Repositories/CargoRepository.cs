using CraneCalc.Application.Interfaces;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CargoRepository(AppDbContext context) : ICargoRepository
{
    public async Task<List<Cargo>> GetCargosPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var cargosPaginated = await context.Cargos
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c=>c.Map())
            .ToListAsync(ct);
        
        return cargosPaginated;
    }

    public async Task<List<Cargo>> GetCargosAsync(CancellationToken ct)
    {
        var cargos = await context.Cargos
            .Select(c=>c.Map())
            .ToListAsync(ct);
        
        return cargos;
    }

    public async Task<List<Cargo>> GetCargosSearchAsync(string search, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(search))
        {
            return await context.Cargos
                .Select(c => c.Map())
                .ToListAsync(ct);
        }

        var cargos = await context.Cargos
            .Where(c => 
                EF.Functions.Like(c.Type, $"%{search}%") || 
                EF.Functions.Like(c.Title, $"%{search}%"))
            .Select(c => c.Map())
            .ToListAsync(ct);
    
        return cargos;
    }

    public async Task<Cargo> GetCargoByIdAsync(Guid id, CancellationToken ct)
    {
        var cargo = await context.Cargos
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        
        if(cargo == null)
            throw new NullReferenceException("Cargo");
        
        return cargo.Map();
    }

    public async Task PutCargoInCartAsync(Guid cargoId, Guid cartId, CancellationToken ct)
    {
        var cargo = await context.Cargos.FirstOrDefaultAsync(c => c.Id == cargoId, ct);
        
        if(cargo == null)
            throw new NullReferenceException("Cargo not found");
        
        var cart = await context.Carts
            .Include(cartEntity => cartEntity.CartCargo)
            .FirstOrDefaultAsync(c => c.Id == cartId, ct);
        
        if(cart == null)
            throw new NullReferenceException("Cart not found");

        if (cart.CartCargo.Any(cartEntity => cartEntity.CargoId == cargoId))
            throw new Exception($"Cart cargo with id {cartId} already exists");
        
        
        cart.CartCargo.Add(new CartCargoEntity
        {
            CartId = cartId,
            CargoId = cargoId,
            Cargo = cargo,
            Cart = cart,
            CalculationResult = 0,
            SafetyComment = string.Empty,
        });
        
        await context.SaveChangesAsync(ct);
    }
}