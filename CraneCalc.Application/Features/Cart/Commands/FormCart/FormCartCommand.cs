using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.FormCart;

public record FormCartCommand(Guid CartId) : IRequest<Domain.Models.Cart?>;