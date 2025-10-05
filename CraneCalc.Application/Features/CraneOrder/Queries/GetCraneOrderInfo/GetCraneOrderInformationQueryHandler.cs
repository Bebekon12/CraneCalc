using CraneCalc.Application.Features.CraneOrder.Dto;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Queries.GetCraneOrderInfo;

public class GetCraneOrderInformationQueryHandler(ICraneOrderRepository repository, IUserService service) : IRequestHandler<GetCraneOrderInformationQuery, CraneOrderInfo?>
{
    public async Task<CraneOrderInfo?> Handle(GetCraneOrderInformationQuery request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        var cart = await repository.GetCraneOrderByUserIdAsync(userId, ct);
        
        if(cart == null)
            return null;
        
        return new CraneOrderInfo
        {
            CartId = cart.Id,
            Quntity = cart.CartCargo.Count
        };
    }
}