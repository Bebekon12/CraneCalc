using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.ModerateCart;

public class ModerateCartCommandHandler(ICartRepository repository, IUserService service) : IRequestHandler<ModerateCartCommand, Domain.Models.CartModel?>
{
    public async Task<Domain.Models.CartModel?> Handle(ModerateCartCommand request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        var cart = await repository.ModerateCartAsync(request.CartId, userId, request.IsApproved, ct);
        
        return cart;
    }
}