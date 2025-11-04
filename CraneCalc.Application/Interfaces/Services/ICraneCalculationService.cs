using CraneCalc.Application.Features.DTOs.Request;
using CraneCalc.Application.Features.DTOs.Response;

namespace CraneCalc.Application.Interfaces.Services;

public interface ICraneCalculationService
{
    Task<CalculationResponse> CalculateCraneProductivityAsync(CalculationRequest request, CancellationToken ct);
}