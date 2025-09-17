using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.User.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository repository, 
    IPasswordHasher hasher) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand request, CancellationToken ct)
    {
        var hashedPassword = hasher.Generate(request.Password);
        
        if(hashedPassword is null)
            throw new EntityException("Invalid password");

        await repository.CreateUserAsync(new UserModel()
        {
            Login = request.Login,
            Password = hashedPassword
        }, ct);
    }
}