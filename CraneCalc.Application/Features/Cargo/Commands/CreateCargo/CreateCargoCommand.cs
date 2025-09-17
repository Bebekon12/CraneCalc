using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
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