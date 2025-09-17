using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.DeleteCargo;

public record DeleteCargoCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteCargoCommandValidator : AbstractValidator<DeleteCargoCommand>
{
    public DeleteCargoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID груза обязателен");
    }
}