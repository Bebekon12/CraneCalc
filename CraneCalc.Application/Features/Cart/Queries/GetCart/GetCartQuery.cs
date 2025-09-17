using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery(Guid Id) : IRequest<Domain.Models.CartModel?>;