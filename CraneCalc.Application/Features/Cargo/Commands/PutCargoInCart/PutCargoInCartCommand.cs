using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCart;

public class PutCargoInCartCommand : IRequest
{
    public Guid CargoId { get; set; }
}