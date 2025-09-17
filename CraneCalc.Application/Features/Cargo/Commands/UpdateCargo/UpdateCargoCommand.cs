using CraneCalc.Application.Features.Cargo.Dto;
using CraneCalc.Application.Features.Shared;
using MediatR;

namespace CraneCalc.Application.Features.Cargo.Commands.UpdateCargo;

public record UpdateCargoCommand : IRequest<CargoResponse?>
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Weight { get; set; }
    
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    
    public double Volume { get; set; }
    public string ConcreteGrade { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}