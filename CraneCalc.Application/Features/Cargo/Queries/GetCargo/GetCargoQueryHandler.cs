using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargo;

public class GetCargoQueryHandler(ICargoRepository repository) : IRequestHandler<GetCargoQuery, Domain.Models.Cargo>
{
    public async Task<Domain.Models.Cargo> Handle(GetCargoQuery request, CancellationToken ct)
    {
        var cargo = await repository.GetCargoByIdAsync(request.CargoId, ct);
        
        return cargo;
    }
}