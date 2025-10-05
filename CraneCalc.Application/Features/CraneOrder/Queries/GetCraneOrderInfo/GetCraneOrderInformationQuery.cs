using CraneCalc.Application.Features.CraneOrder.Dto;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Queries.GetCraneOrderInfo;

public class GetCraneOrderInformationQuery : IRequest<CraneOrderInfo?>;