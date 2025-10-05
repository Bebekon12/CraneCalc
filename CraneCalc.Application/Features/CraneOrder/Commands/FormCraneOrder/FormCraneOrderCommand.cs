using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.FormCraneOrder;

public record FormCraneOrderCommand(Guid CartId) : IRequest<Domain.Models.CraneOrderModel?>;