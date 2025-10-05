using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;

public class UpdateCraneOrderCommandHandler(ICraneOrderRepository repository) : IRequestHandler<UpdateCraneOrderCommand, Domain.Models.CraneOrderModel?>
{
    public async Task<Domain.Models.CraneOrderModel?> Handle(UpdateCraneOrderCommand request, CancellationToken ct)
    {
        var cart = await repository.UpdateCraneOrderAsync(request.CraneOrderId, request, ct);
        
        return cart;
    }
}