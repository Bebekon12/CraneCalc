using System.ComponentModel;
using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Queries.GetCargoPaginated;

public record GetCargosPaginatedQuery : IRequest<PaginatedList<CargoResponse>>
{
    public string? Title { get; init; } = string.Empty;

    public string? Type { get; init; } = string.Empty;

    public double? MinWeight { get; init; } = 0;

    public double? MaxWeight { get; init; } = 0;

    [DefaultValue(1)]
    public int PageNumber { get; init; } = 1;

    [DefaultValue(10)]
    public int PageSize { get; init; } = 10;
}