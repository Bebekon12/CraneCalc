using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CartCargo.Commands.UpdateCartCargo;

public class UpdateCartCargoCommandHandler(ICartCargoRepository repository) : IRequestHandler<UpdateCartCargoCommand, string?>
{
    public async Task<string?> Handle(UpdateCartCargoCommand request, CancellationToken ct)
    {
        var result = await repository.UpdateCargoInCartAsync(request.CartId, request.CargoId, request.SafetyComment, ct);
        
        return result;
    }
}