using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.ModerateCart;

public record ModerateCartCommand(Guid CartId, bool IsApproved) : IRequest<Domain.Models.CartModel?>;