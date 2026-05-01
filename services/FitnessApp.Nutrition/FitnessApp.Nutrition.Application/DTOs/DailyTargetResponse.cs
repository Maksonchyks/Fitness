using FitnessApp.Nutrition.Domain.Enums;

namespace FitnessApp.Nutrition.Application.DTOs
{
    public record DailyTargetResponse(
        Guid Id,
        NutritionGoal Goal,
        NutritionValueDto Target,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
