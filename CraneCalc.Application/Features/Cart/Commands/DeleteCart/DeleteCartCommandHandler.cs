using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Commands.DeleteCart;

public class DeleteCartCommandHandler(ICartRepository repository, IUserService service) : IRequestHandler<DeleteCartCommand>
{
    public async Task Handle(DeleteCartCommand request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        await repository.DeleteCartAsync(request.CartId, userId, ct);
    }
}