using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCart;

public record PutCargoInCartCommand : IRequest
{
    public Guid CargoId { get; set; }
}

public class PutCargoInCartCommandValidator : AbstractValidator<PutCargoInCartCommand>
{
    public PutCargoInCartCommandValidator()
    {
        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("ID груза обязателен");
    }
}