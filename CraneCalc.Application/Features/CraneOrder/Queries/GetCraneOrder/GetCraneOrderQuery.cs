using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Queries.GetCraneOrder;

public record GetCraneOrderQuery(Guid Id) : IRequest<Domain.Models.CraneOrderModel?>;