using MediatR;

namespace CraneCalc.Application.Features.CraneCargo.Commands.DeleteCargoInCraneOrder;

public record DeleteCargoInCraneOrderCommand(Guid CartId, Guid CargoId) : IRequest<string?>;