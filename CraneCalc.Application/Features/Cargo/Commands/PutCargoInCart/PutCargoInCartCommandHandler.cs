using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCart;

public class PutCargoInCartCommandHandler(ICargoRepository repository) : IRequestHandler<PutCargoInCartCommand>
{
    public async Task Handle(PutCargoInCartCommand request, CancellationToken ct)
    {
        await repository.PutCargoInCartAsync(request.CargoId, ct);
    }
}