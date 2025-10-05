using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CraneCargo.Commands.UpdateCraneCargo;

public class UpdateCraneOrderCargoCommandHandler(ICraneCargoRepository repository) : IRequestHandler<UpdateCraneOrderCargoCommand, string?>
{
    public async Task<string?> Handle(UpdateCraneOrderCargoCommand request, CancellationToken ct)
    {
        var result = await repository.UpdateCargoInCraneOrderAsync(request.CartId, request.CargoId, request.SafetyComment, ct);
        
        return result;
    }
}