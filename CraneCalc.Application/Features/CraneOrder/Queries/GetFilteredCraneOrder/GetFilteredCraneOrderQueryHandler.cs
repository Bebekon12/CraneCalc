using CraneCalc.Application.Features.CraneOrder.Dto;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Queries.GetFilteredCraneOrder;

public class GetFilteredCraneOrderQueryHandler(ICraneOrderRepository repository, IUserRepository userRepository) : IRequestHandler<GetFilteredCraneOrderQuery, List<CraneOrderListDto>>
{
    public async Task<List<CraneOrderListDto>> Handle(GetFilteredCraneOrderQuery request, CancellationToken ct)
    {
        var carts = await repository.GetFilteredCraneOrderAsync(request.From, request.Before, request.Status, ct);

        var mapper = new CraneOrderListMapper(userRepository);
        
        var mappedList = carts.Select(c=>mapper.Map(c)).ToList();
        
        return mappedList;
    }
}

public class CraneOrderListMapper(IUserRepository userRepository)
{
    public CraneOrderListDto Map(CraneOrderModel model)
    {
        return new CraneOrderListDto
        {
            CalculationResult = model.CalculationResult,
            Id = model.Id,
            Status = model.Status,
            CreatedDate = model.CreatedDate,
            Creator = userRepository.GetUserByIdAsync(model.CreatorId, CancellationToken.None).Result!.Login,
            Moderator = model.ModeratorId == null
                ? null
                : userRepository.GetUserByIdAsync(model.ModeratorId.Value, CancellationToken.None).Result!.Login,
            FormationDate = model.FormationDate,
            LoadCapacity = model.LoadCapacity,
            LiftingHeight = model.LiftingHeight,
            JibOutreach = model.JibOutreach,
            LiftingSpeed = model.LiftingSpeed,
            CartCargo = model.CartCargo,
        };
    }
}