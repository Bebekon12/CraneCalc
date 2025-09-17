using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.DeleteCargo;

public class DeleteCargoCommand : IRequest
{
    public Guid Id { get; set; }
}