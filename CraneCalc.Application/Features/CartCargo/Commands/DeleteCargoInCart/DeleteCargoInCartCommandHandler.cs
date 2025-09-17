using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CartCargo.Commands.DeleteCargoInCart;

public class DeleteCargoInCartCommandHandler(ICartCargoRepository repository) : IRequestHandler<DeleteCargoInCartCommand, string?>
{
    public async Task<string?> Handle(DeleteCargoInCartCommand request, CancellationToken ct)
    {
        var result = await repository.DeleteCargoInCartAsync(request.CartId, request.CargoId, ct);
        
        return result;
    }
}