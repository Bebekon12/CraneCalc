using AutoMapper;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class UserRepository(AppDbContext context, IMapper mapper) : IUserRepository
{
    public async Task<User> CreateUserAsync(User user, CancellationToken ct)
    {
        var entityUser = mapper.Map<UserEntity>(user);
        
        await context.Users.AddAsync(entityUser, ct);
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<User>(entityUser);
    }

    public async Task<User?> GetUserAsync(int userId, CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u=>u.Id==userId, ct);
        
        return user == null 
            ? null 
            : mapper.Map<User>(user);
    }

    public async Task<User?> UpdateUserAsync(int userId, User user, CancellationToken ct)
    {
        var entityUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if(entityUser == null)
            return null;
        
        if(entityUser.Login != user.Login)
            entityUser.Login = user.Login;
        
        if(entityUser.Password != user.Password)
            entityUser.Password = user.Password;
        
        if(entityUser.Role != user.Role)
            entityUser.Role = user.Role;
        
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<User>(entityUser);
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
            throw new EntityException("User not found");
        
        return mapper.Map<User>(user);
    }
}