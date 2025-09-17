using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;

namespace CraneCalc.Infrastructure.EntityMappers;

public static class UserEntityMapper
{
    public static User Map(this UserEntity model)
    {
        return new User
        {
            Id = model.Id,
            Login = model.Login,
            Password = model.Password,
            Role = model.Role,
        };
    }
    
    public static UserEntity Map(this User model)
    {
        return new UserEntity
        {
            Id = model.Id,
            Login = model.Login,
            Password = model.Password,
            Role = model.Role,
        };
    }
}