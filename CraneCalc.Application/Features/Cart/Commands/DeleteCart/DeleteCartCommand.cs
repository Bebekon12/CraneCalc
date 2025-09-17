using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.DeleteCart;

public record DeleteCartCommand(Guid CartId): IRequest;