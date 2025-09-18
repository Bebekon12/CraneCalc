using CraneCalc.Application.Features.Cart.Dto;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetCartInfo;

public class GetCartInformationQueryHandler(ICartRepository repository, IUserService service) : IRequestHandler<GetCartInformationQuery, CartInfo?>
{
    public async Task<CartInfo?> Handle(GetCartInformationQuery request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        var cart = await repository.GetCartByUserIdAsync(userId, ct);
        
        if(cart == null)
            return null;
        
        return new CartInfo
        {
            CartId = cart.Id,
            Quntity = cart.CartCargo.Count
        };
    }
}