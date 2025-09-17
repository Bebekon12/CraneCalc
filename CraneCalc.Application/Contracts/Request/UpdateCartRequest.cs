namespace CraneCalc.Application.Contracts.Request;

public record UpdateCartRequest(
    double LoadCapacity,
    double LiftingHeight,
    double JibOutreach,
    double LiftingSpeed);