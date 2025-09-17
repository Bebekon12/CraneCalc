using MediatR;

namespace CraneCalc.Application.Features.CartCargo.Commands.DeleteCargoInCart;

public record DeleteCargoInCartCommand(Guid CartId, Guid CargoId) : IRequest<string?>;