using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<UserModel> CreateUserAsync(UserModel userModel, CancellationToken ct);
    Task<UserModel?> GetUserByIdAsync(Guid userId, CancellationToken ct);
    Task<UserModel?> GetUserByLoginAsync(string login, CancellationToken ct);
    Task<UserModel?> UpdateUserAsync(Guid userId, UserModel userModel, CancellationToken ct);
}