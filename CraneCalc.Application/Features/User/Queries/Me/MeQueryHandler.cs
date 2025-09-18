using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.User.Queries.Me;

public class MeQueryHandler(IUserRepository repository, IUserService service) : IRequestHandler<MeQuery, UserModel?>
{
    public async Task<UserModel?> Handle(MeQuery request, CancellationToken ct)
    {
        var login = service.GetCurrentUserLogin();
        
        if (login == null)
            throw new EntityException("login is null");
        
        var user = await repository.GetUserByLoginAsync(login, ct);
        
        return user;
    }
}