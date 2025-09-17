using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.FormCart;

public class FormCartCommandHandler(ICartRepository repository) : IRequestHandler<FormCartCommand, Domain.Models.Cart?>
{
    public async Task<Domain.Models.Cart?> Handle(FormCartCommand request, CancellationToken ct)
    {
        var cart = await repository.FormCartAsync(request.CartId, ct);
        
        return cart;
    }
}