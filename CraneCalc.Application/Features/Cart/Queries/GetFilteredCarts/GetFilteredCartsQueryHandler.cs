using CraneCalc.Application.Features.Cart.Dto;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.Cart.Queries.GetFilteredCarts;

public class GetFilteredCartsQueryHandler(ICartRepository repository, IUserRepository userRepository) : IRequestHandler<GetFilteredCartsQuery, List<CartListDto>>
{
    public async Task<List<CartListDto>> Handle(GetFilteredCartsQuery request, CancellationToken ct)
    {
        var carts = await repository.GetFilteredCartsAsync(request.From, request.Before, request.Status, ct);

        var mapper = new CartListMapper(userRepository);
        
        var mappedList = carts.Select(c=>mapper.Map(c)).ToList();
        
        return mappedList;
    }
}

public class CartListMapper(IUserRepository userRepository)
{
    public CartListDto Map(CartModel model)
    {
        return new CartListDto
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