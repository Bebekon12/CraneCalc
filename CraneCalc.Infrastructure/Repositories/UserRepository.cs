using AutoMapper;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class UserRepository(AppDbContext context, IMapper mapper) : IUserRepository
{
    public async Task<UserModel> CreateUserAsync(UserModel userModel, CancellationToken ct)
    {
        var entityUser = mapper.Map<UserEntity>(userModel);
        
        await context.Users.AddAsync(entityUser, ct);
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<UserModel>(entityUser);
    }

    public async Task<UserModel?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u=>u.Id==userId, ct);
        
        return user == null 
            ? null 
            : mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> GetUserByLoginAsync(string login, CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Login == login, ct);
        
        return user == null
            ? null
            : mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> UpdateUserAsync(int userId, UserModel userModel, CancellationToken ct)
    {
        var entityUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if(entityUser == null)
            return null;
        
        if(entityUser.Login != userModel.Login)
            entityUser.Login = userModel.Login;
        
        if(entityUser.Password != userModel.Password)
            entityUser.Password = userModel.Password;
        
        if(entityUser.Role != userModel.Role)
            entityUser.Role = userModel.Role;
        
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<UserModel>(entityUser);
    }
}