using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Domain.Enums;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.SetDailyTarget
{
    public record SetDailyTargetCommand(
        NutritionGoal Goal,
        float Calories,
        float Proteins,
        float Fats,
        float Carbs
    ) : IRequest<DailyTargetResponse>;
}
