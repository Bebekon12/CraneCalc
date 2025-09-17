using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.AddImageToCargo;

public record AddCargoImageCommand(
    Guid CargoId,
    Stream FileStream) : IRequest<string>;
    
public class AddCargoImageCommandValidator : AbstractValidator<AddCargoImageCommand>
{
    public AddCargoImageCommandValidator()
    {
        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("ID груза обязателен");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("Файл изображения обязателен");
    }
}