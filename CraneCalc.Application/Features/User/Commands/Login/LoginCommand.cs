using CraneCalc.Application.Features.User.Dto;
using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.User.Commands.Login;

public class LoginCommand : IRequest<AuthenticationResult?>
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не должен быть пустым")
            .MinimumLength(3).WithMessage("Логин должен содержать минимум 3 символа")
            .MaximumLength(50).WithMessage("Логин должен содержать не более 50 символов");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не должен быть пустым")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
            .MaximumLength(100).WithMessage("Пароль должен содержать не более 100 символов");
    }
}