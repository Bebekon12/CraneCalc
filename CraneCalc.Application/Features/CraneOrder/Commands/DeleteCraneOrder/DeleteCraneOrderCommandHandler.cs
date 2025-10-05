using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.DeleteCraneOrder;

public class DeleteCraneOrderCommandHandler(ICraneOrderRepository repository, IUserService service) : IRequestHandler<DeleteCraneOrderCommand>
{
    public async Task Handle(DeleteCraneOrderCommand request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        await repository.DeleteCraneOrderAsync(request.CartId, userId, ct);
    }
}