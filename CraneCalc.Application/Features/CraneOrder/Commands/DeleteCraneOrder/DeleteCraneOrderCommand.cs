using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.DeleteCraneOrder;

public record DeleteCraneOrderCommand(Guid CartId): IRequest;