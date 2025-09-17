using CraneCalc.Application.Features.Cargo.Dto;
using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.CreateCargo;

public record CreateCargoCommand : IRequest<CargoResponse>
{
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public double Weight { get; init; }
    
    public double Length { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    
    public double Volume { get; init; }
    public string ConcreteGrade { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class CreateCargoCommandValidator : AbstractValidator<CreateCargoCommand>
{
    public CreateCargoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название груза обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Тип груза обязателен")
            .MaximumLength(50).WithMessage("Тип не должен превышать 50 символов");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Вес должен быть положительным числом")
            .LessThan(100000).WithMessage("Вес не может превышать 100 000 кг");

        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Длина должна быть положительной")
            .LessThan(1000).WithMessage("Длина не может превышать 1000 м");

        RuleFor(x => x.Width)
            .GreaterThan(0).WithMessage("Ширина должна быть положительной")
            .LessThan(1000).WithMessage("Ширина не может превышать 1000 м");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Высота должна быть положительной")
            .LessThan(1000).WithMessage("Высота не может превышать 1000 м");

        RuleFor(x => x.Volume)
            .GreaterThan(0).WithMessage("Объем должен быть положительным")
            .LessThan(1000000).WithMessage("Объем не может превышать 1 000 000 м³");

        RuleFor(x => x.ConcreteGrade)
            .MaximumLength(20).WithMessage("Марка бетона не должна превышать 20 символов");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов");
    }
}