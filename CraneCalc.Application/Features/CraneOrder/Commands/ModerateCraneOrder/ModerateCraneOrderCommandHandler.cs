using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.ModerateCraneOrder;

public class ModerateCraneOrderCommandHandler(ICraneOrderRepository repository, IUserService service) : IRequestHandler<ModerateCraneOrderCommand, Domain.Models.CraneOrderModel?>
{
    public async Task<Domain.Models.CraneOrderModel?> Handle(ModerateCraneOrderCommand request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        var cart = await repository.ModerateCraneOrderAsync(request.CraneOrderId, userId, request.IsApproved, ct);
        
        return cart;
    }
}