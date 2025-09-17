using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.UpdateCart;

public record UpdateCartCommand(
    Guid CartId, 
    double LoadCapacity, 
    double LiftingHeight, 
    double JibOutreach, 
    double LiftingSpeed) : IRequest<Domain.Models.CartModel?>;