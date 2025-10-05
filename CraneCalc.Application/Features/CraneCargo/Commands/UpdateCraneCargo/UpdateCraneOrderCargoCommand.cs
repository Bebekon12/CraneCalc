using MediatR;

namespace CraneCalc.Application.Features.CraneCargo.Commands.UpdateCraneCargo;

public record UpdateCraneOrderCargoCommand(Guid CartId, Guid CargoId, string SafetyComment) : IRequest<string?>;