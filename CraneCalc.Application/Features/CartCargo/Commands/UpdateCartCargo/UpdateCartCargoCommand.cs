using MediatR;

namespace CraneCalc.Application.Features.CartCargo.Commands.UpdateCartCargo;

public record UpdateCartCargoCommand(Guid CartId, Guid CargoId, string SafetyComment) : IRequest<string?>;