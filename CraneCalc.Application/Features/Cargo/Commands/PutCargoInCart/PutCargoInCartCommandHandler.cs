using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCart;

public class PutCargoInCartCommandHandler(ICargoRepository repository, IUserService service) : IRequestHandler<PutCargoInCartCommand>
{
    public async Task Handle(PutCargoInCartCommand request, CancellationToken ct)
    {
        var user = await service.GetCurrentUserAsync(ct);
        
        if(user == null)
            throw new EntityException("User does not exist");
        
        await repository.PutCargoInCartAsync(request.CargoId, user.Id, user.Role == Role.Moderator, ct);
    }
}