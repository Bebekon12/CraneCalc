using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargo;

public class GetCargoQueryHandler(ICargoRepository repository) : IRequestHandler<GetCargoQuery, Domain.Models.CargoModel>
{
    public async Task<Domain.Models.CargoModel> Handle(GetCargoQuery request, CancellationToken ct)
    {
        var cargo = await repository.GetCargoByIdAsync(request.CargoId, ct);
        
        return cargo;
    }
}