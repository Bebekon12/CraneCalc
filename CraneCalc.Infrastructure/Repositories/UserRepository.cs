using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.EntityMappers;
using Microsoft.EntityFrameworkCore;

namespace CraneCalc.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> CreateUserAsync(User user, CancellationToken ct)
    {
        var entityUser = user.Map();
        
        await context.Users.AddAsync(entityUser, ct);
        await context.SaveChangesAsync(ct);
        
        return entityUser.Map();
    }

    public async Task<User?> GetUserAsync(int userId, CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u=>u.Id==userId, ct);
        
        return user?.Map();
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
        
        return entityUser.Map();
    }
}