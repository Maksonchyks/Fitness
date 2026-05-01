namespace FitnessApp.Nutrition.Application.DTOs
{
    public record WeightLogResponse(
        Guid Id,
        float Weight,
        DateTime LoggedAt
    );
}
