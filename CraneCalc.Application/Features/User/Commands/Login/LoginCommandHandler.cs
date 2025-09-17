using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Exceptions;
using MediatR;

namespace CraneCalc.Application.Features.User.Commands.Login;

public class LoginCommandHandler(
    IUserRepository repository, 
    IJwtProvider provider, 
    IPasswordHasher hasher) : IRequestHandler<LoginCommand, string?>
{
    public async Task<string?> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await repository.GetUserByLoginAsync(request.Login, ct);
        
        if(user == null)
            throw new EntityException("User not found");
        
        var isCorrectUser = hasher.Verify(request.Password, user.Password);
        
        if(!isCorrectUser)
            throw new EntityException("Failed to login");
        
        var token = provider.GenerateToken(user);
        
        return token;
    }
}