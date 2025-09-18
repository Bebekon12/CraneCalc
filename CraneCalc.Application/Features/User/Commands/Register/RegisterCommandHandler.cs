using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CraneCalc.Application.Features.User.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository repository, 
    IPasswordHasher hasher,
    IConfiguration configuration) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand request, CancellationToken ct)
    {
        var hashedPassword = hasher.Generate(request.Password);
        
        if(hashedPassword is null)
            throw new EntityException("Invalid password");
        
        var admin = configuration.GetSection("AdminUser").Value;
        
        await repository.CreateUserAsync(new UserModel
        {
            Login = request.Login,
            Password = hashedPassword,
            Role = request.Login == admin ? Role.Moderator : Role.User,
        }, ct);
    }
}