using FitnessApp.Nutrition.Domain.Enums;

namespace FitnessApp.Nutrition.Application.DTOs
{
    public record TemplateMealResponse(
        MealType MealType,
        string DishName,
        string Description,
        NutritionValueDto Nutrition
    );

    public record MealPlanTemplateResponse(
        Guid Id,
        string Name,
        string Description,
        NutritionValueDto TotalNutrition,
        List<TemplateMealResponse> Meals,
        int RemainingTemplates
    );
}
