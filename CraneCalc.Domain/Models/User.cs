using CraneCalc.Domain.Enums;

namespace CraneCalc.Domain.Models;

public class User
{
    public int Id { get; set; }

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.User;
}