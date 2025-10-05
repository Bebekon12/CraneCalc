using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.FormCraneOrder;

public record FormCraneOrderCommand(Guid CraneOrderId) : IRequest<Domain.Models.CraneOrderModel?>;