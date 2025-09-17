using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.UpdateCart;

public class UpdateCartCommandHandler(ICartRepository repository) : IRequestHandler<UpdateCartCommand, Domain.Models.CartModel?>
{
    public async Task<Domain.Models.CartModel?> Handle(UpdateCartCommand request, CancellationToken ct)
    {
        var cart = await repository.UpdateCartAsync(request.CartId, request, ct);
        
        return cart;
    }
}