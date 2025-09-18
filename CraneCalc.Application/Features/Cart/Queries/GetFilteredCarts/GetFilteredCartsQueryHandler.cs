using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetFilteredCarts;

public class GetFilteredCartsQueryHandler(ICartRepository repository) : IRequestHandler<GetFilteredCartsQuery, List<CartModel>>
{
    public async Task<List<CartModel>> Handle(GetFilteredCartsQuery request, CancellationToken ct)
    {
        var carts = await repository.GetFilteredCartsAsync(request.From, request.Before, ct);
        
        return carts;
    }
}