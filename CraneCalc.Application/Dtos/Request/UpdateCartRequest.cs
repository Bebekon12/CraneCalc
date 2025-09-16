namespace CraneCalc.Application.Dtos.Request;

public record UpdateCartRequest(
    double LoadCapacity,
    double LiftingHeight,
    double JibOutreach,
    double LiftingSpeed);