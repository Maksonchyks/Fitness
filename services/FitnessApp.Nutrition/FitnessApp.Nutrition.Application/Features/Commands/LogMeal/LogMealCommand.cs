using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Domain.Enums;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.LogMeal
{
    public record LogMealCommand(
        string MealName,
        MealType MealType,
        float Calories,
        float Proteins,
        float Fats,
        float Carbs
    ) : IRequest<MealLogResponse>;
}
