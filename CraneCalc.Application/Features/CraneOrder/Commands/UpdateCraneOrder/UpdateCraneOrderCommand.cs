using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;

public record UpdateCraneOrderCommand(
    Guid CraneOrderId, 
    double LoadCapacity, 
    double LiftingHeight, 
    double JibOutreach, 
    double LiftingSpeed) : IRequest<Domain.Models.CraneOrderModel?>;