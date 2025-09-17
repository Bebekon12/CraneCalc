using CraneCalc.Application.Features.Cart.Dto;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetCartInfo;

public class GetCartInformationQuery : IRequest<CartInfo?>;