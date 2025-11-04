using System.Text.Json.Serialization;

namespace CraneCalc.Domain.Models;


public class CargoCalculationResult
{
    [JsonPropertyName("cargoId")]
    public string CargoId { get; set; } = string.Empty;

    [JsonPropertyName("calculationResult")]
    public double? CalculationResult { get; set; }
}