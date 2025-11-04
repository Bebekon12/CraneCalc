using System.Text.Json.Serialization;

namespace CraneCalc.Application.Features.DTOs.Response;

public class CalculationResponse
{
    [JsonPropertyName("crane_order_id")]
    public string CraneOrderId { get; set; }
    
    [JsonPropertyName("is_success")]
    public bool IsSuccess { get; set; }
    
    [JsonPropertyName("total_calculation_result")]
    public double? TotalCalculationResult { get; set; }
    
    [JsonPropertyName("cargo_results")]
    public List<CargoResult> CargoResults { get; set; } = new();
}

public class CargoResult
{
    [JsonPropertyName("cargo_id")]
    public string CargoId { get; set; }
    
    [JsonPropertyName("calculation_result")]
    public double? CalculationResult { get; set; }
}