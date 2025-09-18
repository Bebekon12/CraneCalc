using CraneCalc.Application.Features.User.Dto;
using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Domain.Exceptions;
using MediatR;

namespace CraneCalc.Application.Features.User.Commands.Login;

public class LoginCommandHandler(
    IUserRepository repository, 
    IJwtProvider provider, 
    IPasswordHasher hasher,
    ITokenStorage  tokenStorage) : IRequestHandler<LoginCommand, AuthenticationResult?>
{
    public async Task<AuthenticationResult?> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await repository.GetUserByLoginAsync(request.Login, ct);
        
        if(user == null)
            throw new EntityException("User not found");
        
        var isCorrectUser = hasher.Verify(request.Password, user.Password);
        
        if(!isCorrectUser)
            throw new EntityException("Failed to login");
        
        var accessToken = provider.GenerateToken(user);
        var refreshToken = provider.GenerateRefreshToken();
        
        await tokenStorage.SaveRefreshTokenAsync(
            user.Id, 
            refreshToken, 
            TimeSpan.FromDays(7), 
            ct
        );
        
        return new AuthenticationResult(accessToken, refreshToken);
    }
}