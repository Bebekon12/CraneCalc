using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetCart;

public class GetCartQueryHandler(ICartRepository repository) : IRequestHandler<GetCartQuery, Domain.Models.Cart?>
{
    public async Task<Domain.Models.Cart?> Handle(GetCartQuery request, CancellationToken ct)
    {
        var cart = await repository.GetCartByIdAsync(request.Id, ct);
        
        return cart ?? null;
    }
}