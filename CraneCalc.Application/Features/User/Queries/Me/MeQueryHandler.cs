using CraneCalc.Application.Features.User.Dto;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using MediatR;

namespace CraneCalc.Application.Features.User.Queries.Me;

public class MeQueryHandler(IUserRepository repository, IUserService service) : IRequestHandler<MeQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(MeQuery request, CancellationToken ct)
    {
        var login = service.GetCurrentUserLogin();
        
        if (login == null)
            throw new EntityException("login is null");
        
        var user = await repository.GetUserByLoginAsync(login, ct);
        
        if(user == null)
            throw new EntityException("user is null");
        
        return new UserResponse(user.Login, user.Role==Role.User?"User":"Moderator");
    }
}