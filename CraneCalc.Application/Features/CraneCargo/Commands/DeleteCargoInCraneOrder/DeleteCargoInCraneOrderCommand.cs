using MediatR;

namespace CraneCalc.Application.Features.CraneCargo.Commands.DeleteCargoInCraneOrder;

public record DeleteCargoInCraneOrderCommand(Guid CraneOrderId, Guid CargoId) : IRequest<string?>;