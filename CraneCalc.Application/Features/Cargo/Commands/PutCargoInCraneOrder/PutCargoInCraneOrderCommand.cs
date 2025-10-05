using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCraneOrder;

public record PutCargoInCraneOrderCommand : IRequest
{
    public Guid CargoId { get; set; }
}

public class PutCargoInCartCommandValidator : AbstractValidator<PutCargoInCraneOrderCommand>
{
    public PutCargoInCartCommandValidator()
    {
        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("ID груза обязателен");
    }
}