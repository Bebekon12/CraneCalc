using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;

public record UpdateCraneOrderCommand(
    Guid CartId, 
    double LoadCapacity, 
    double LiftingHeight, 
    double JibOutreach, 
    double LiftingSpeed) : IRequest<Domain.Models.CraneOrderModel?>;