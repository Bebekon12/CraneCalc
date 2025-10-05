using CraneCalc.Application.Features.CraneOrder.Dto;
using CraneCalc.Domain.Enums;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Queries.GetFilteredCraneOrder;

public record GetFilteredCraneOrderQuery(DateTime From, DateTime Before, Status Status) : IRequest<List<CraneOrderListDto>>;