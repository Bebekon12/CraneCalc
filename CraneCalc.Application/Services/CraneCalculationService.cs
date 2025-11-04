using System.Net.Http.Json;
using System.Text.Json;
using CraneCalc.Application.Features.DTOs.Request;
using CraneCalc.Application.Features.DTOs.Response;
using CraneCalc.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CraneCalc.Application.Services;

public class CraneCalculationService : ICraneCalculationService
{
    private readonly HttpClient _httpClient;
    private const string FastApiBaseUrl = "http://localhost:8000";
    private readonly ILogger<CraneCalculationService> _logger;

    public CraneCalculationService(HttpClient httpClient, ILogger<CraneCalculationService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(FastApiBaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _logger = logger;
    }

    public async Task<CalculationResponse> CalculateCraneProductivityAsync(CalculationRequest request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Initiating calculation for order {OrderId}", request.CraneOrderId);
            
            var response = await _httpClient.PostAsJsonAsync("/calculate", request, ct);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Calculation initiated successfully for order {OrderId}", 
                    request.CraneOrderId);
                
                // Возвращаем пустой ответ, так как результаты придут через callback
                return new CalculationResponse 
                { 
                    CraneOrderId = request.CraneOrderId,
                    IsSuccess = true 
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("FastAPI returned error {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
                
                return new CalculationResponse 
                { 
                    CraneOrderId = request.CraneOrderId,
                    IsSuccess = false 
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling FastAPI service for order {OrderId}", 
                request.CraneOrderId);
            
            return new CalculationResponse 
            { 
                CraneOrderId = request.CraneOrderId,
                IsSuccess = false 
            };
        }
    }
}