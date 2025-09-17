using AutoMapper;
using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.UpdateCargo;

public class UpdateCargoCommandHandler(ICargoRepository repository, IMapper mapper) : IRequestHandler<UpdateCargoCommand, CargoResponse?>
{
    public async Task<CargoResponse?> Handle(UpdateCargoCommand request, CancellationToken ct)
    {
        var updatedCargo = await repository.UpdateCargoAsync(
            request.Id,
            request,
            ct);

        return updatedCargo == null 
            ? null 
            : mapper.Map<CargoResponse>(updatedCargo);
    }
}