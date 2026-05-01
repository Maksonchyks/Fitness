using FitnessApp.Nutrition.Domain.Enums;

namespace FitnessApp.Nutrition.Application.DTOs
{
    public record MealLogResponse(
        Guid Id,
        string MealName,
        MealType MealType,
        NutritionValueDto Nutrition,
        DateTime LoggedAt
    );
}
