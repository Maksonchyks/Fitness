namespace FitnessApp.Nutrition.Application.DTOs
{
    public record DailyProgressResponse(
        NutritionValueDto Consumed,
        NutritionValueDto? Target,
        NutritionProgressDto? Progress,
        NutritionAdjustmentDto? Adjustment,
        List<MealLogResponse> Meals
    );

    public record NutritionProgressDto(
        float CaloriesPercent,
        float ProteinsPercent,
        float FatsPercent,
        float CarbsPercent
    );

    public record NutritionAdjustmentDto(
        string Message,
        NutritionValueDto SuggestedTarget,
        float AdjustmentPercent
    );
}
