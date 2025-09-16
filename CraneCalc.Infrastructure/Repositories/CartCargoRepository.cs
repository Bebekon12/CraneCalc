using CraneCalc.Application.Interfaces;
using CraneCalc.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CartCargoRepository(AppDbContext context) : ICartCargoRepository
{
    public async Task<string?> DeleteCargoInCartAsync(Guid cartId, Guid cargoId, CancellationToken ct)
    {
        var cartCargo = await context.CartCargos
            .FirstOrDefaultAsync(c=>c.CartId==cartId && c.CargoId==cargoId && !c.IsDeleted, ct);
        
        if(cartCargo == null)
            return null;
        
        cartCargo.IsDeleted = true;
        await context.SaveChangesAsync(ct);
        return "success";
    }

    public async Task<string?> UpdateCargoInCartAsync(Guid cargoId, Guid cartId, string safetyComment, CancellationToken ct)
    {
        var cartCargo = await context.CartCargos
            .FirstOrDefaultAsync(c=>c.CartId==cartId && c.CargoId==cargoId && !c.IsDeleted, ct);
        
        if(cartCargo == null)
            return null;
        
        cartCargo.SafetyComment = safetyComment;
        await context.SaveChangesAsync(ct);
        return safetyComment;
    }
}