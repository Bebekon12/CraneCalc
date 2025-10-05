using CraneCalc.Application.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class CraneCargoRepository(AppDbContext context) : ICraneCargoRepository
{
    public async Task<string?> DeleteCargoInCraneOrderAsync(Guid craneOrderId, Guid cargoId, CancellationToken ct)
    {
        var cartCargo = await context.CraneCargos
            .FirstOrDefaultAsync(c=>c.CraneOrderId==craneOrderId && c.CargoId==cargoId, ct);
        
        if(cartCargo == null)
            return null;
        
        await context.SaveChangesAsync(ct);
        return "success";
    }

    public async Task<string?> UpdateCargoInCraneOrderAsync(Guid cargoId, Guid craneOrderId, string safetyComment, CancellationToken ct)
    {
        var cartCargo = await context.CraneCargos
            .FirstOrDefaultAsync(c=>c.CraneOrderId==craneOrderId && c.CargoId==cargoId, ct);
        
        if(cartCargo == null)
            return null;
        
        cartCargo.SafetyComment = safetyComment;
        await context.SaveChangesAsync(ct);
        return safetyComment;
    }
}