namespace CraneCalc.Application.Contracts.Request;

public class UpdateUserRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}