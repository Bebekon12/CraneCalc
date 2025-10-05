using MediatR;

namespace CraneCalc.Application.Features.CraneCargo.Commands.UpdateCraneCargo;

public record UpdateCraneOrderCargoCommand(Guid CraneOrderId, Guid CargoId, string SafetyComment) : IRequest<string?>;