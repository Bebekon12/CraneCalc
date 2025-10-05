using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCraneOrder;

public class PutCargoInCraneOrderCommandHandler(ICargoRepository repository, IUserService service) : IRequestHandler<PutCargoInCraneOrderCommand>
{
    public async Task Handle(PutCargoInCraneOrderCommand request, CancellationToken ct)
    {
        var user = await service.GetCurrentUserAsync(ct);
        
        if(user == null)
            throw new EntityException("User does not exist");
        
        await repository.PutCargoInCraneOrderAsync(request.CraneOrderId, user.Id, user.Role == Role.Moderator, ct);
    }
}