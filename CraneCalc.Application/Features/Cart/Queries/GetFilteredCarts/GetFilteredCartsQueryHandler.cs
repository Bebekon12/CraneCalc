using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetFilteredCarts;

public class GetFilteredCartsQueryHandler(ICartRepository repository) : IRequestHandler<GetFilteredCartsQuery, List<Domain.Models.Cart>>
{
    public async Task<List<Domain.Models.Cart>> Handle(GetFilteredCartsQuery request, CancellationToken ct)
    {
        var carts = await repository.GetFilteredCartsAsync(request.From, request.Before, ct);
        
        return carts;
    }
}