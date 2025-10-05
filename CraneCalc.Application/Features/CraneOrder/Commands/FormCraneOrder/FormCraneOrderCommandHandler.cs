using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.FormCraneOrder;

public class FormCraneOrderCommandHandler(ICraneOrderRepository repository) : IRequestHandler<FormCraneOrderCommand, Domain.Models.CraneOrderModel?>
{
    public async Task<Domain.Models.CraneOrderModel?> Handle(FormCraneOrderCommand request, CancellationToken ct)
    {
        var cart = await repository.FormCraneOrderAsync(request.CraneOrderId, ct);
        
        return cart;
    }
}