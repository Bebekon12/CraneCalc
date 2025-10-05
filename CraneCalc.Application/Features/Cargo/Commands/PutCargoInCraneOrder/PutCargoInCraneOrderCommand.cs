using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.PutCargoInCraneOrder;

public record PutCargoInCraneOrderCommand : IRequest
{
    public Guid CraneOrderId { get; set; }
}

public class PutCargoInCartCommandValidator : AbstractValidator<PutCargoInCraneOrderCommand>
{
    public PutCargoInCartCommandValidator()
    {
        RuleFor(x => x.CraneOrderId)
            .NotEmpty().WithMessage("ID груза обязателен");
    }
}