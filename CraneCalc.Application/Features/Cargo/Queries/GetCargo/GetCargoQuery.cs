using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargo;

public record GetCargoQuery : IRequest<Domain.Models.Cargo>
{
    public Guid CargoId { get; set; }
}

public class GetCargoQueryValidator : AbstractValidator<GetCargoQuery>
{
    public GetCargoQueryValidator()
    {
        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("ID груза обязателен");
    }
}