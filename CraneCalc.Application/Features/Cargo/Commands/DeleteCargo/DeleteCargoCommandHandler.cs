using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.DeleteCargo;

public class DeleteCargoCommandHandler(ICargoRepository repository) : IRequestHandler<DeleteCargoCommand>
{
    public async Task Handle(DeleteCargoCommand request, CancellationToken ct)
    {
        await repository.DeleteCargoAsync(request.Id, ct);
    }
}