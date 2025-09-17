using AutoMapper;
using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;

public class GetCargosPaginatedQueryHandler
    (ICargoRepository repository, 
        IMapper mapper) : IRequestHandler<GetCargosPaginatedQuery, PaginatedList<CargoResponse>>
{
    public async Task<PaginatedList<CargoResponse>> Handle(GetCargosPaginatedQuery request, CancellationToken ct)
    {
        var cargos = await repository.GetCargosPaginatedAsync(
            request,
            request.PageNumber,
            request.PageSize,
            ct);

        var cargoDtos = mapper.Map<List<CargoResponse>>(cargos);
        
        return new PaginatedList<CargoResponse>(
            cargoDtos,
            cargos.Count,
            request.PageNumber,
            request.PageSize);
    }
}