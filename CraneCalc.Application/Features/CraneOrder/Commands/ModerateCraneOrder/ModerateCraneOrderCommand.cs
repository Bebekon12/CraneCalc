using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.ModerateCraneOrder;

public record ModerateCraneOrderCommand(Guid CartId, bool IsApproved) : IRequest<Domain.Models.CraneOrderModel?>;