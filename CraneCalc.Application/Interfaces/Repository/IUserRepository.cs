using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<UserModel> CreateUserAsync(UserModel userModel, CancellationToken ct);
    Task<UserModel?> GetUserByIdAsync(int userId, CancellationToken ct);
    Task<UserModel?> GetUserByLoginAsync(string login, CancellationToken ct);
    Task<UserModel?> UpdateUserAsync(int userId, UserModel userModel, CancellationToken ct);
}