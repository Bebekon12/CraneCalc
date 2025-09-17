using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.ModerateCart;

public class ModerateCartCommandHandler(ICartRepository repository) : IRequestHandler<ModerateCartCommand, Domain.Models.Cart?>
{
    public async Task<Domain.Models.Cart?> Handle(ModerateCartCommand request, CancellationToken ct)
    {
        var cart = await repository.ModerateCartAsync(request.CartId, 1, request.IsApproved, ct);
        
        return cart;
    }
}