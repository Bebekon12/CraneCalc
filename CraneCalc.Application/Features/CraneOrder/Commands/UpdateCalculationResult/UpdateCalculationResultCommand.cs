using System.Text.Json.Serialization;
using CraneCalc.Domain.Models;
using MediatR;

namespace CraneCalc.Application.Features.CraneOrder.Commands.UpdateCalculationResult;

public class UpdateCalculationResultCommand : IRequest<bool>
{
    [JsonPropertyName("craneOrderId")]
    public string CraneOrderId { get; set; } = string.Empty;

    [JsonPropertyName("totalCalculationResult")]
    public double? TotalCalculationResult { get; set; }

    [JsonPropertyName("cargoResults")]
    public List<CargoCalculationResult> CargoResults { get; set; } = new();

    [JsonPropertyName("authToken")]
    public string AuthToken { get; set; } = string.Empty;
}