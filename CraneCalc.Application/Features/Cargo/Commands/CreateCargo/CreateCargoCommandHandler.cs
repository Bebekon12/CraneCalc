using AutoMapper;
using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.CreateCargo;

public class CreateCargoCommandHandler(ICargoRepository repository, IMapper mapper) : IRequestHandler<CreateCargoCommand, CargoResponse>
{
    public async Task<CargoResponse> Handle(CreateCargoCommand request, CancellationToken ct)
    {
        var createdCargo = await repository.CreateCargoAsync(mapper.Map<Domain.Models.CargoModel>(request), ct);
        
        return mapper.Map<CargoResponse>(createdCargo);
    }
}