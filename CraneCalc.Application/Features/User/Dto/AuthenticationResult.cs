namespace CraneCalc.Application.Features.User.Dto;

public record AuthenticationResult(
    string AccessToken,
    string RefreshToken
);