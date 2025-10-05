using CraneCalc.Application.Features.CraneOrder.Commands.UpdateCraneOrder;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Repository;

public interface ICraneOrderRepository
{
    Task<CraneOrderModel?> GetCraneOrderByIdAsync(Guid craneOrderId, CancellationToken ct);
    Task<CraneOrderModel?> GetCraneOrderByUserIdAsync(Guid userId, CancellationToken ct);
    Task<List<CraneOrderModel>> GetFilteredCraneOrderAsync(DateTime from, DateTime before, Status status, CancellationToken ct);
    Task<CraneOrderModel?> UpdateCraneOrderAsync(Guid id, UpdateCraneOrderCommand craneOrder, CancellationToken ct);
    Task<CraneOrderModel?> FormCraneOrderAsync(Guid craneOrderId, CancellationToken ct);
    Task<CraneOrderModel?> ModerateCraneOrderAsync(Guid craneOrderId, Guid userId, bool isApproved, CancellationToken ct);
    Task<string?> DeleteCraneOrderAsync(Guid craneOrderId, Guid userId, CancellationToken ct);
}