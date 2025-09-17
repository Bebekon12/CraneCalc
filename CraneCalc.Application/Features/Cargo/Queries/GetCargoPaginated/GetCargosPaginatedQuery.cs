using System.ComponentModel;
using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using FluentValidation;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;

public record GetCargosPaginatedQuery : IRequest<PaginatedList<CargoResponse>>
{
    public string? Title { get; init; } = string.Empty;

    public string? Type { get; init; } = string.Empty;

    public double? MinWeight { get; init; }

    public double? MaxWeight { get; init; }

    [DefaultValue(1)]
    public int PageNumber { get; init; } = 1;

    [DefaultValue(10)]
    public int PageSize { get; init; } = 10;
}

public class GetCargosPaginatedQueryValidator : AbstractValidator<GetCargosPaginatedQuery>
{
    public GetCargosPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Номер страницы должен быть положительным");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100");

        RuleFor(x => x.MinWeight)
            .GreaterThan(0).When(x => x.MinWeight.HasValue)
            .WithMessage("Минимальный вес должен быть положительным");

        RuleFor(x => x.MaxWeight)
            .GreaterThan(0).When(x => x.MaxWeight.HasValue)
            .WithMessage("Максимальный вес должен быть положительным")
            .GreaterThanOrEqualTo(x => x.MinWeight.Value)
            .When(x => x.MinWeight.HasValue && x.MaxWeight.HasValue)
            .WithMessage("Максимальный вес должен быть больше или равен минимальному");
    }
}