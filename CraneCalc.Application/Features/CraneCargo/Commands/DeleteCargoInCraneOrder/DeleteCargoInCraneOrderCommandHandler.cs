using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CraneCargo.Commands.DeleteCargoInCraneOrder;

public class DeleteCargoInCraneOrderCommandHandler(ICraneCargoRepository repository) : IRequestHandler<DeleteCargoInCraneOrderCommand, string?>
{
    public async Task<string?> Handle(DeleteCargoInCraneOrderCommand request, CancellationToken ct)
    {
        var result = await repository.DeleteCargoInCraneOrderAsync(request.CraneOrderId, request.CargoId, ct);
        
        return result;
    }
}