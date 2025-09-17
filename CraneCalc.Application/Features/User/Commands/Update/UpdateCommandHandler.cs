using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.User.Commands.Update;

public class UpdateCommandHandler(
    IUserRepository repository, 
    IJwtProvider provider, 
    IPasswordHasher hasher) : IRequestHandler<UpdateCommand, string?>
{
    public async Task<string?> Handle(UpdateCommand request, CancellationToken ct)
    {
        var passwordHash = hasher.Generate(request.Password);

        if(passwordHash == null)
            throw new EntityException("Invalid password");
        
        var updatedUser = await repository.UpdateUserAsync(request.Id, new UserModel
        {
            Login = request.Login,
            Password = passwordHash,
        }, ct);
        
        if(updatedUser == null)
            throw new EntityException("Invalid login or password");
        
        var token = provider.GenerateToken(updatedUser);
        
        return token;
    }
}