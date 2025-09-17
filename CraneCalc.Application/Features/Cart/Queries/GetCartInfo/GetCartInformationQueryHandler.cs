using CraneCalc.Application.Features.Cart.Dto;
using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetCartInfo;

public class GetCartInformationQueryHandler(ICartRepository repository) : IRequestHandler<GetCartInformationQuery, CartInfo?>
{
    public async Task<CartInfo?> Handle(GetCartInformationQuery request, CancellationToken ct)
    {
        var cart = await repository.GetCartByUserIdAsync(1, ct);
        
        if(cart == null)
            return null;
        
        return new CartInfo
        {
            CartId = cart.Id,
            Quntity = cart.CartCargo.Count
        };
    }
}