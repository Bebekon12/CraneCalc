using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargo;

public record GetCargoQuery : IRequest<Domain.Models.Cargo>
{
    public Guid CargoId { get; set; }
}