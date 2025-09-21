using CraneCalc.Application.Features.Cart.Dto;
using CraneCalc.Domain.Enums;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetFilteredCarts;

public record GetFilteredCartsQuery(DateTime From, DateTime Before, Status Status) : IRequest<List<CartListDto>>;