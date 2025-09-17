using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.DeleteCart;

public class DeleteCartCommandHandler(ICartRepository repository) : IRequestHandler<DeleteCartCommand>
{
    public async Task Handle(DeleteCartCommand request, CancellationToken ct)
    {
        await repository.DeleteCartAsync(request.CartId, 1, ct);
    }
}