using CraneCalc.Application.Interfaces.Repository;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Queries.GetCraneOrder;

public class GetCraneOrderQueryHandler(ICraneOrderRepository repository) : IRequestHandler<GetCraneOrderQuery, Domain.Models.CraneOrderModel?>
{
    public async Task<Domain.Models.CraneOrderModel?> Handle(GetCraneOrderQuery request, CancellationToken ct)
    {
        var cart = await repository.GetCraneOrderByIdAsync(request.Id, ct);
        
        return cart ?? null;
    }
}