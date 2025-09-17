using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user, CancellationToken ct);
    Task<User?> GetUserAsync(int userId, CancellationToken ct);
    Task<User?> UpdateUserAsync(int userId, User user, CancellationToken ct);
}